function initMap() {
    var uluru = { lat: parseFloat($('#lat').val()), lng: parseFloat($('#lng').val()) };
    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 15,
        center: uluru
    });
    var marker = new google.maps.Marker({
        position: uluru,
        map: map
    });
}
