

window.addEventListener('beforeunload', function (event) {
    $.ajax({
        type: 'POST',
        url: '/Home/Logoff',
        dataType: 'html',
        success: alert("logout")
    });
});