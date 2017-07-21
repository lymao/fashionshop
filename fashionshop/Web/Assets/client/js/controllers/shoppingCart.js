var cart = {
    init: function () {
        cart.loadData();
        cart.registerEvents();
    },

    registerEvents: function () {
        $('.btnAddToCart').off('click').on('click', function (e) {
            e.preventDefault();
            var quantity = 0;
            var sizeId = 0;
            quantity = $('#quantityDetail').text();
            var productId = parseInt($(this).data('id'));
            sizeId = parseInt($('#myselect').val());
            cart.addItem(productId, quantity, sizeId);
        });

        $('select').change(function () {
            var sizeIdSlected = 0;
            var productId = 0;
            sizeIdSlected = $(this).find('option:selected').val();
            productId = parseInt($(this).data('id'));
            cart.updateSize(sizeIdSlected, productId);
        });

        $('.simpleCart_empty').off('click').on('click', function (e) {
            e.preventDefault();
            cart.deleteAll();
        });

        //--quantity Detail page
        $('.value-plus1').off('click').on('click', function () {
            var divUpd = $(this).parent().find('.value1'), newVal = parseInt(divUpd.text(), 10) + 1;
            divUpd.text(newVal);
        });
        $('.value-minus1').off('click').on('click', function () {
            var divUpd = $(this).parent().find('.value1'), newVal = parseInt(divUpd.text(), 10) - 1;
            if (newVal >= 1) divUpd.text(newVal);
        });
        //--quantity Detail page

        $('.btnDeleteItem').off('click').on('click', function (e) {
            e.preventDefault();
            var productId = parseInt($(this).data('id'));
            cart.deleteItem(productId);
        });

        $('.txtQuantity').off('change').on('change', function () {
            var productId = parseInt($(this).data('id'));
            var quantity = parseInt($(this).val());
            var price = parseFloat($(this).data('price'));
            if (isNaN(quantity) === false) {
                var amount = quantity * price;

                $('#amount_' + productId).text(numeral(amount).format('0,0'));

            }
            else {
                $('#amount_' + productId).text(0);
            }

            $('#lblTotalOrder').text(numeral(cart.getTotalOrder()).format('0,0'));

            cart.updateAll();

        });

        $('#btnDeleteAll').off('click').on('click', function (e) {
            e.preventDefault();
            cart.deleteAll();
        });

        $('#btnContinue').off('click').on('click', function (e) {
            e.preventDefault();
            window.location.href = '/';
        });

        $('#btnCheckout').off('click').on('click', function (e) {
            e.preventDefault();
            $('#divCheckout').toggle();
        });

        $('#chkUserLoginInfo').off('click').on('click', function () {
            if ($(this).prop('checked')) {
                cart.getUserLogin();
            } else {
                $('#name').val('');
                $('#address').val('');
                $('#email').val('');
                $('#phone').val('');
            }
        });

        $('#btnCreateOrder').off('click').on('click', function (e) {
            e.preventDefault();
            var isvalid = $("#frmCheckout").valid();
            if (isvalid)
                cart.createOrder();
        });

        // validation jquery
        $("#frmCheckout").validate({
            rules: {
                name: 'required',
                address: 'required',
                message:'required',
                email: {
                    required: true,
                    email: true
                },
                phone: {
                    required: true,
                    number: true
                }
            },
            messages: {
                name: "Bạn chưa nhập tên",
                address: "Yêu cầu nhập địa chỉ",
                email: {
                    required: "Bạn chưa nhập email",
                    email: "Định dạng email chưa đúng"
                },
                phone: {
                    required: "Bạn chưa nhập số điện thoại",
                    number: "Số điện thoại phải là số."
                },
                message:'Bạn phải nhập tin nhắn'
            }
        });
    },

    addItem: function (productId, quantity, sizeId) {
        $.ajax({
            url: '/ShoppingCart/Add',
            type: 'POST',
            dataType: 'json',
            data: {
                productId: productId,
                quanlity: quantity,
                sizeId: sizeId
            },
            success: function (res) {
                if (res.status) {
                    alert('Thêm vào giỏ hàng thành công.');
                    cart.loadData();
                } else {
                    alert(res.message);
                    cart.loadData();
                }

            }
        });
    },

    updateSize: function (sizeIdSlected, productId) {
        $.ajax({
            url: '/ShoppingCart/UpdateSize',
            type: 'POST',
            dataType: 'json',
            data: {
                sizeIdSlected: sizeIdSlected,
                productId: productId
            },
            success: function (result) {
                if (result.status)
                    cart.loadData();
            }
        })
    },


    createOrder: function () {
        var order = {
            CustomerName: $('#name').val(),
            CustomerAddress: $('#address').val(),
            CustomerEmail: $('#email').val(),
            CustomerMobile: $('#phone').val(),
            CustomerMessage: $('#message').val(),
            PaymentMethod: "Thanh toán tiền mặt",
            Status: false
        }
        $.ajax({
            url: '/ShoppingCart/CreateOrder',
            type: 'POST',
            dataType: 'json',
            data: {
                orderViewModel: JSON.stringify(order)
            },
            success: function (res) {
                if (res.status) {
                    cart.deleteAll();
                    setTimeout(function () {
                        $('#cartContent').html('<h4 style ="color:blue; text-align:center;">Chúc mừng bạn đã đặt hàng thành công! Chúng tôi sẽ liên hệ với bạn sớm nhất.</h4>');
                    }, 2000);
                    $('#divCheckout').hide();
                }
                else {
                    $('#showMessage').show();
                    $('#showMessage').html('<h4 style="text-align:center;color:red;">' + res.message + '</h4>');
                }
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

    getUserLogin: function () {
        $.ajax({
            url: '/ShoppingCart/GetUserLogin',
            type: 'POST',
            dataType: 'json',
            success: function (res) {
                if (res.status) {
                    var user = res.data;
                    $('#name').val(user.FullName);
                    $('#address').val(user.Address);
                    $('#email').val(user.Email);
                    $('#phone').val(user.PhoneNumber);
                }
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
                if (res.status) {
                    cart.loadData();
                    $('#showMessage').html("");
                    $('#showMessage').hide();
                }

            }
        });
    },

    deleteAll: function () {
        $.ajax({
            url: '/ShoppingCart/DeleteAll',
            type: 'POST',
            dataType: 'json',
            success: function (res) {
                if (res.status) {
                    cart.loadData();
                }

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
                    $('#cartQuantity').text(data.length);
                    var rendered = '';
                    var template = $('#template').html();
                    $.each(data, function (i, item) {
                        if (item.Size !== null) {
                            rendered += Mustache.render(template, {
                                ProductId: item.ProductId,
                                ProductName: item.Product.Name,
                                Image: item.Product.Image,
                                SizeId: item.Size.ID,
                                SizeName: item.Size.Name,
                                ProductSize: item.Product.ProductSizes,
                                Price: item.Product.Price,
                                PriceF: numeral(item.Product.Price).format('0,0'),
                                Quantity: item.Quantity,
                                Amount: numeral(item.Product.Price * item.Quantity).format('0,0')
                            });
                        }
                        else {
                            rendered += Mustache.render(template, {
                                ProductId: item.ProductId,
                                ProductName: item.Product.Name,
                                Image: item.Product.Image,
                                ProductSize: item.Product.ProductSizes,
                                Price: item.Product.Price,
                                PriceF: numeral(item.Product.Price).format('0,0'),
                                Quantity: item.Quantity,
                                Amount: numeral(item.Product.Price * item.Quantity).format('0,0')
                            });
                        }

                    });
                    if (rendered === '')
                        $('#cartContent').html('<div align="center"><h4 style="color:blue;">Giỏ hàng của bạn còn trống.</h4><br /> <button class="btn btn-block" style="color:blue;"><a href="/">Tiếp tục mua hàng</a></button></div>');
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