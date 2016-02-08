$("#content").ready(function () {
    GetRecentPosts();
});

$("#search").keyup(function () {
    var key = $("#search").val();

    if (key.length > 0) {
        var url = "/Home/PostSearch";
        $.post(url, { keyword: key })
         .done(function (table) {
             $("#content").empty();
             $("#content").append(table);
             $("#title").empty();
             $("#title").append("Search Results");
         });
    } else {
        GetRecentPosts();
    }
});

function GetRecentPosts() {
    var url = "/Home/RecentPosts";
    $.post(url, { count: 8 })
     .done(function (table) {
         $("#content").empty();
         $("#content").append(table);
         $("#title").empty();
         $("#title").append("Recent Posts");
     });
}