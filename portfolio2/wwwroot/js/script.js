$('#createstudentform').keyup(function () {
    var phone = document.getElementById("phone");
    var year = document.getElementById("year");
    var good_color = "#66cc66";
    var bad_color = "#ff6666";
    var warning = $('#warning');
    var warning1 = $('#warning1');
    if (phone.val().length == 8) {
        phone.css('background-color', good_color);
        warning.css('color', good_color).html("Correct!");
    }
    else {
        phone.css('background-color', bad_color);
        warning.css('color', bad_color).html("Phone number needs to be at least 8 digit!");
    }
    if (year.val() == 1 || year.val() == 2 || year.val() == 3) {
        year.css('background-color', good_color);
        warning1.css('color', bad_color).html("Good!");
    }
});