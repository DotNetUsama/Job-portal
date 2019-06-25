// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

var setNotificationDialogPosition = () => {
    var notificationsButton = $("#notifications");
    var notificationsDialog = $(".notification-dialog");
    var notificationsButtonPosition = notificationsButton.offset();
    var notificationsDialogWidth = notificationsDialog.width();
    var notificationsButtonWidth = notificationsButton.width() + 30;
    var notificationsButtonHeight = notificationsButton.height() + 18;
    var top = notificationsButtonPosition.top + notificationsButtonHeight;
    var left = notificationsButtonPosition.left - notificationsDialogWidth + notificationsButtonWidth;
    $(".notification-dialog").css({ top: top, left: left });
};

var showNotifications = notifications => {
    var dialog = $(".notification-dialog");
    var list = $(".notification-dialog > ul");
    list.html("");

    dialog.css({ "display": "block" });

    notifications.forEach(notification => appendNotificationToList(list, notification));

    appendFooterToNotificationList(list);

    $("#notifications-number").html(0);
    $("#notifications-number").hide();
};

var clickNotification = (url, id) => {
    $.post({
        url: `/Notifications/ReadNotification?id=${id}`,
        success: function(data) {
            console.log(data);
            window.location.href = url;
        },
        error: function(err) {
            console.log(err);
        }
    });
};

var appendNotificationToList = (list, notification) => {
    var type = $("<span></span>");
    type.addClass("notification-type");
    type.html(notification["type"]);
    var time = $("<span></span>");
    time.addClass("notification-time");
    time.html(notification["createdAt"]);
    var header = $("<div></div>");
    header.addClass("notification-header");
    header.append(type);
    header.append(time);
    var anchor = $("<a></a>");
    anchor.attr("href", "javascript:;");
    anchor.addClass("notification-anchor");
    anchor.html(notification["message"]);
    anchor.on("click", () => {
        clickNotification(notification["url"], notification["id"]);
    });
    var content = $("<div></div>");
    content.addClass("notification-content");
    content.append(anchor);
    var listItem = $("<li></li>");
    listItem.addClass("notification-area");
    if (!notification["isRead"]) {
        listItem.addClass("not-read");
    }
    listItem.append(header);
    listItem.append(content);
    list.append(listItem);
};

var appendFooterToNotificationList = (list) => {
    var anchor = $("<a></a>");
    anchor.attr("href", "/");
    anchor.html("See all your inbox");
    var listItem = $("<li></li>");
    listItem.addClass("notification-area");
    listItem.addClass("dialog-footer");
    listItem.append(anchor);
    list.append(listItem);
};

// Write your Javascript code.
$(document).ready(()=> {

    $(window).resize(setNotificationDialogPosition);

    $(".js-relative-time").timeago();

    $.ajax({
        url: "/Notifications/GetNotificationsCount",
        method: "POST",
        success: function (data) {
            $("#notifications-number").html(data);
            if (data === 0)
                $("#notifications-number").hide();
        }
    });

    var connection = new signalR.HubConnectionBuilder().withUrl("/signalRHub").build();

    connection.on("receiveNotification", () => {
        $("#notifications-number").html(parseInt($("#notifications-number").html()) + 1);
        $("#notifications-number").show();
    });

    connection.start();
    
    $(document).mouseup(e => {
        var container = $(".notification-dialog");
        if (!container.is(e.target) && container.has(e.target).length === 0) {
            container.hide();
            $("#notifications").css({ "background-color": "initial" });
        }
    });

    $("#notifications").click(() => {
        setNotificationDialogPosition();
        $("#notifications").css({ "background-color": "#dbe8f5" });
        $.ajax({
            url: "/Notifications/GetNotifications",
            method: "POST",
            dataType: "json",
            success: function (data) {
                showNotifications(data);
            },
            error: function(err) {
                console.log(err);
            }
        });
    });
});