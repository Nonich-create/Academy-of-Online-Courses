/* 
this javascript is only to change the "actpage" attribut on the .cdp div
*/
jQueryAjaxGet = (page) => {

    window.onload = function () {
        var paginationPage = parseInt($('.cdp').attr('actpage'), 10);
        $('.cdp_i').on('click', function () {
            paginationPage = parseInt(page, 10);

            $('.cdp').attr('actpage', paginationPage);
        });
    };
}
