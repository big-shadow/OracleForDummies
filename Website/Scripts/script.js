$("#search").keyup(function () {
    var key = $("#search").val();

    if (key.length > 0) {
        var url = "/Home/PostSearch";
        $.post(url, { keyword: key })
         .done(function (table) {
             $("#posts").empty();
             $("#posts").append(table);
         });
    } else {
        var url = "/Home/RecentPosts";
        $.post(url, { count: 3 })
         .done(function (table) {
             $("#posts").empty();
             $("#posts").append(table);
         });
    }
});