var Main = (function ($, _, Backbone, signalr, undefined) {

	var self = {};
	var _debug = true;
	var _baseApiUrl;
	var _chatClient;
	var _chat = signalr.mainHub;
	var _members;
	var _messages;
	var _msgDisplay;

	var App = {
		Models: {},
		Collections: {},
		Views: {},
		Events: {}
	}

	App.Models = App.Models || {};
	App.Collections = App.Collections || {};
	App.Views = App.Views || {};
	App.Events = App.Events || {};

	App.Events.UserJoined = _.extend({}, Backbone.Events);
	App.Events.UserLeft = _.extend({}, Backbone.Events);

	App.Models.Error = Backbone.Model.extend();
	App.Models.Message = Backbone.Model.extend();
	App.Models.Member = Backbone.Model.extend();


	App.Collections.MessageCollection = Backbone.Collection.extend({
		model: App.Models.Message,
		url: '/messages'
	});
	App.Collections.MemberCollection = Backbone.Collection.extend({
		model: App.Models.Member,
		url: function () {
			return _baseApiUrl + "/Members"
		}
	});

	App.Views.ErrorView = Backbone.View.extend({
		template: _.template($("#error-tmpl").html()),
		initialize: function () {
			_log("Initializing Error View");
		},
		render: function () {
			this.$el.empty();
			this.$el.append(this.template(this.model.toJSON()));
			return this;
		},
		events: {
			"click .btn-close": "OnClose"
		},
		OnClose: function (e) {
			e.preventDefault();
			this.remove();
		}
	});

	App.Views.Empty = Backbone.View.extend({
		viewName: "Empty",
		tagName: "div",
		className: "",
		template: _.template($("#").html()),
		initialize: function () {
			_log("Initializing " + this.viewName + " View");
		},
		render: function () {
			this.$el.empty().append(this.template(this.model.toJSON()));
			return this;
		},
		events: {
		}
	});

	App.Views.Chat = Backbone.View.extend({
		viewName: "Chat",
		tagName: "ul",
		className: "chats",
		template: _.template($("#chat-msg-tmpl").html()),
		initialize: function () {
			_log("Initializing " + this.viewName + " View");
			this.listenTo(_messages, "add", this.renderMessage);
			this.listenTo(App.Events.UserJoined, "user:joined", this.addStatusMessage)
			this.listenTo(App.Events.UserLeft, "user:left", this.addStatusMessage)
		},
		render: function () {
			//			this.$el.empty().append(this.template());
			return this;
		},
		renderMessage: function (model) {
			_log(model);
			var _self = this;
			var cls = "by-other";
			if (model.get('FromUser') === _chatClient.UserName) {
				cls = "by-me";
			}
			var touser = model.get('ToUser');
			if (touser !== null && touser.length > 0) {
				cls = "by-private";
			}
			_self.$el.append($(_self.template(model.toJSON())).addClass(cls));

			_self.scrollToBottom();

			//	Remove Stale chat messages
			while (_self.$el.children().length > 200) {
				_self.$el.children('li:first').remove();
			}
		},
		addStatusMessage: function (model) {
			var _self = this;
			var tmpl = _.template($("#status-msg-tmpl").html());
			_self.$el.append($(tmpl(model)));
			_self.scrollToBottom();
		},
		scrollToBottom: function () {
			//	Scroll to the bottom
			$('.chatWindow').scrollTop($('.chatWindow')[0].scrollHeight);
		}
	});

	App.Views.Members = Backbone.View.extend({
		viewName: "Members",
		tagName: "ul",
		className: "members",
		template: _.template($("#member-tmpl").html()),
		initialize: function () {
			_log("Initializing " + this.viewName + " View");
			this.listenTo(_members, "reset", this.renderMembers);
			this.listenTo(_members, "add", this.renderMembers);
			this.listenTo(_members, "remove", this.renderMembers);
		},
		render: function () {
			return this;
		},
		events: {
		},
		renderMembers: function () {
			_log("Loading Members");
			var _self = this;
			_self.$el.empty();
			_members.forEach(function (member) {
				_log(member.toJSON());
				_self.$el.append(_self.template(member.toJSON()));
			});
		}
	});

	App.Views.ChatInput = Backbone.View.extend({
		viewName: "ChatInput",
		tagName: "div",
		className: "",
		template: _.template($("#chatInput-tmpl").html()),
		initialize: function () {
			_log("Initializing " + this.viewName + " View");
		},
		render: function () {
			this.$el.empty().append(this.template());
			return this;
		},
		events: {
			"click .btn": "OnSendMessage",
			"keyup": "OnKeyUp"
		},
		OnSendMessage: function () {
			var msg = $("#chat-input-txt").val();
			signalr.mainHub.server.sendMessage(_chatClient.UserName, msg);
			$("#chat-input-txt").val("").focus();
		},
		OnKeyUp: function (e) {
			_self = this;
			if (e.which === 13) {
				_self.OnSendMessage();
			}
		}
	});

	var _log = (function (obj) {
		if (_debug) {
			if (window['console'] != undefined) {
				console.log(obj);
			}
		}
	});

	var buildError = (function (title, description, status, xhr) {
		var errorModel = { title: title, statusText: description, status: status, data: null };
		try {
			if (xhr != null) {
				var mr = JSON.parse(xhr.responseText);
				errorModel.data = mr;
			}
		} catch (err) { }
		return errorModel;
	});

	var addClient = (function (chatClient) {
		_members.add(chatClient);
		App.Events.UserJoined.trigger("user:joined", { UserName: chatClient.UserName, Status: "Joined" });
	});

	var removeClient = (function (chatClient) {
		_log(chatClient);
		var m = _members.find(function (member) {
			return member.get('ConnectionId') == chatClient.ConnectionId;
		});
		_members.remove(m);
		App.Events.UserLeft.trigger("user:left", { UserName: chatClient.UserName, Status: "Left" });
	});
	//	addChatMessage
	//	addClient
	//	removeClient
	var setupSignalR = (function () {

		signalr.mainHub.client.addClient = (function (chatClient) {
			_log(chatClient);
			addClient(chatClient);
		});
		signalr.mainHub.client.removeClient = (function (chatClient) {
			_log(chatClient);
			removeClient(chatClient);
		});
		signalr.mainHub.client.addChatMessage = (function (msg) {
			_log(msg);
			_messages.add(msg);
		});

		var srp = signalr.hub.start();
		srp.done(function () {
			_log("Connection Started");

			signalr.mainHub.server.joinChat(_chatClient.UserName, _chatClient.AvatarClass);
			var p = _members.fetch({ reset: true });
			p.done(function () {
				_log("finished loading members " + _members.length);
			});
		});
		srp.fail(function () {
			_log("Connection failed");
		});
	});

	var initialize = (function () {
		_log("Initializing MainChat");
		_members = new App.Collections.MemberCollection();
		_messages = new App.Collections.MessageCollection();
		//	Start the App Here
		_msgDisplay = new App.Views.Chat().render().$el.appendTo(".chatWindow");
		new App.Views.ChatInput().render().$el.appendTo(".chatInput");
		new App.Views.Members().render().$el.appendTo(".memberList");
		$("#chat-input-txt").focus();

		setupSignalR();

	})();

	self.setChatClient = (function (chatclient) {
		_chatClient = chatclient;
	});

	self.refreshMembers = (function () {
		_members.fetch({ reset: true });
	});

	self.setApiUrl = (function (url) {
		_baseApiUrl = url;
	});

	self.shutDown = (function () {
		signalr.hub.stop();
	});

	return self;

});