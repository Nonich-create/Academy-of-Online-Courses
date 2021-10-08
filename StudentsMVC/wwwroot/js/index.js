/* 
this javascript is only to change the "actpage" attribut on the .cdp div
*/
//jQueryAjaxGet = (url, page) => {

//    window.onload = function () {
//        var paginationPage = parseInt($('.cdp').attr('actpage'), 10);
 
//        var searchString = $('.test').val();
//        var serachParameter = $('.test2').val();
//        $.ajax({
//          //  url: url,
//            data: "searchString=" + searchString + "&serachParameter=" + serachParameter + "&page=" + page,
//            type: "POST",
//            error: function (passParams) {
//                alert("Error" + searchString);
//            }
//        });
//        $('.cdp_i').on('click', function () {
//            var go = page;
//            paginationPage = parseInt(go, 10);

//            $('.cdp').attr('actpage', paginationPage);
//        });
//    };
//}
jQueryAjaxGet = (url, page) => {
    var paginationPage = parseInt($('.cdp').attr('actpage'), 10);
    $('.cdp_i').on('click', function () {
        var go = page;
        if (go === '+1') {
            paginationPage++;
        } else if (go === '-1') {
            paginationPage--;
        } else {
            paginationPage = parseInt(go, 10);
        }
        $('.cdp').attr('actpage', paginationPage);
    });

    var searchString = $('.test').val();
    var serachParameter = $('.test2').val();
    $.ajax({
        url: url,
        data: "searchString=" + searchString + "&serachParameter=" + serachParameter + "&page=" + page,
        type: "POST",
        //success: function (data) {
        //    alert(searchString);
        //},
        error: function (passParams) {
            alert("Error" + searchString);
        }
    });
}

