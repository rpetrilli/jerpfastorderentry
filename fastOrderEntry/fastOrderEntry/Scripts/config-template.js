(function () {
    $(document).ready(function () {
        // -- SubMenu  --------------------------------//
        $('[data-submenu]').submenupicker();
    });
}).call(this);

// -- Shortcut  --------------------------------//
shortcut.add("Ctrl+Alt+V", function () {
    window.location.href =  $('#mnu_ordini_vend').attr("href"); 
});
shortcut.add("Ctrl+Alt+A", function () {
    window.location.href = $('#mnu_ordini_acq').attr("href");
});

shortcut.add("F9", function (e) {
    var input = 'input#' + e.target.id;
    $(input).val(function (index, val) {
        return val + '%';
    });
    e.preventDefault();
});