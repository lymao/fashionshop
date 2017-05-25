var common = {
    init: function () {
        common.registerEvents();
    },
    registerEvents: function () {
        var availableTags = function (request, response) {
            $.ajax({
                url: '/Product/GetListProductByName',
                dataType: 'json',
                data: {
                    keyword: request.term
                },
                success: function (res) {
                    response(res.data);
                }
            });
        }

        $("#txtKeyword").autocomplete({
            source: availableTags
        });
    }
}
common.init();