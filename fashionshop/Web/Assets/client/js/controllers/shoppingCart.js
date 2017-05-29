var cart = {
    init: function () {
        cart.loadData();
        cart.registerEvents();
    },
    registerEvents: function () {
        $('.btnAddToCart').off('click').on('click', function (e) {
            e.preventDefault();
            var productId = parseInt($(this).data('id'));
            cart.addItem(productId);
        });

        $('.btnDeleteItem').off('click').on('click', function (e) {
            e.preventDefault();
            var productId = parseInt($(this).data('id'));
            cart.deleteItem(productId);
        });

        $('.txtQuantity').off('change').on('change', function () {
            var productId = parseInt($(this).data('id'));
            var quantity = parseInt($(this).val());
            var price = parseFloat($(this).data('price'));
            if (isNaN(quantity) == false) {
                var amount = quantity * price;
                $('#amount_' + productId).text(numeral(amount).format('0,0'));
            } else {
                $('#amount_' + productId).text(0);
            }
            cart.updateAll();
        });


    },

    addItem: function (productId) {
        $.ajax({
            url: '/ShoppingCart/Add',
            type: 'POST',
            dataType: 'json',
            data: {
                productId: productId
            },
            success: function (res) {
                if (res.status)
                    alert('Thêm vào giỏ hàng thành công.');
                cart.loadData();
            }
        });
    },

    updateAll: function () {
        var cartlist = [];
        $.each($('.txtQuantity'), function (i, item) {
            cartlist.push({
                ProductId: $(item).data('id'),
                Quantity: $(item).val()
            });
        });
        $.ajax({
            url: '/ShoppingCart/Update',
            type: 'POST',
            dataType: 'json',
            data: {
                cartData: JSON.stringify(cartlist)
            },
            success: function (res) {
                if (res.status)
                    cart.loadData();
            }
        });

    },


    deleteItem: function (productId) {
        $.ajax({
            url: '/ShoppingCart/Delete',
            type: 'POST',
            dataType: 'json',
            data: {
                productId: productId
            },
            success: function (res) {
                if (res.status)
                    cart.loadData();
            }
        });
    },

    loadData: function () {
        $.ajax({
            url: '/ShoppingCart/GetAll',
            type: 'GET',
            dataType: 'json',
            success: function (res) {
                if (res.status) {
                    data = res.data;
                    // show quanlity on Cart
                    $('#cartQuantity').text(data.length);
                    var rendered = '';
                    var template = $('#template').html();
                    $.each(data, function (i, item) {
                        rendered += Mustache.render(template, {
                            ProductId: item.ProductId,
                            ProductName: item.Product.Name,
                            Image: item.Product.Image,
                            Price: item.Product.Price,
                            PriceF: numeral(item.Product.Price).format('0,0'),
                            Quantity: item.Quantity,
                            Amount: numeral(item.Product.Price * item.Quantity).format('0,0')

                        });
                    });
                    if (rendered == '')
                        $('.cart-items').html('<h2 style="text-align:center;color:blue;">Không có sản phẩm nào trong giỏ hàng.</h2>');
                    $('#cartBody').html(rendered);
                    $('#lblTotalOrder').text(numeral(cart.getTotalOrder()).format('0,0'));
                   
                    cart.registerEvents();
                }
            }
        });
    },

    getTotalOrder: function () {
        var listTextbox = $('.txtQuantity');
        var total = 0;
        $.each(listTextbox, function (i, item) {
            total += parseInt($(item).val()) * parseFloat($(item).data('price'));
        });
        return total;
    }
}
cart.init();