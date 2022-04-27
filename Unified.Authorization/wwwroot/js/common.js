var commonfunctionality = function () {
    var getmessage = function () {
        if ($("#englishLanguage").length > 0) {
            return "الحقل إجباري";
        }
        else return "This field is mandatory";
    };

    var showspinner = function () {
        $('#spinner').removeClass('dn').addClass('db');
    };

    var hidespinner = function () {
        $('#spinner').removeClass('db').addClass('dn');
    };

    return {
        showmessage: getmessage,
        showspinner: showspinner,
        hidespinner: hidespinner
    }
}();

