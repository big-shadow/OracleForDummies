var key = "";

$("#content").ready(function () {
    GetRecentPosts();
});

$("#search").keyup(function () {
    key = $("#search").val();
    GetPostsByKey();
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

function GetPostsByKey() {
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
}