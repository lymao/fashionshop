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

        $('.simpleCart_empty').off('click').on('click', function (e) {
            e.preventDefault();
            cart.deleteAll();
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
                }
            }
        });
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
                    $('#showMessage').html('<h4 style="text-align:center;color:blue;">' + res.message + '</h4>');
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

    addItem: function (productId) {
        $.ajax({
            url: '/ShoppingCart/Add',
            type: 'POST',
            dataType: 'json',
            data: {
                productId: productId
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
                        $('#cartContent').html('<h2 style="text-align:center;color:blue;">Không có sản phẩm trong giỏ hàng.</h2>');
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