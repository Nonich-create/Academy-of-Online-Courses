/* 
this javascript is only to change the "actpage" attribut on the .cdp div
*/

window.onload = function(){
  
};

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
        type: "GET",
        //success: function (data) {
        //    alert(searchString);
        //},
        error: function (passParams) {
            alert("Error" + searchString);
        }
    });
}

