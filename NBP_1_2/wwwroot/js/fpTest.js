
    var user = '@HttpContext.Session.GetString("userType")';
    if(user != "Admin"){
        document.getElementById("Edit").disabled = true;
    }
