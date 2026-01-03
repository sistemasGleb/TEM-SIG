var Exportar = function () {



    var _componentForm = function () {
 
    };

    return {
        init: function () {
            _componentForm();
        }
    };
}();

document.addEventListener('DOMContentLoaded', function () {
    Exportar.init();
});


function Export() {
    html2canvas($("#map_basic"), {
        useCORS: true,
        onrendered: function (canvas) {
            $("#imgMap").attr("src", canvas.toDataURL("image/png"));
            $("#imgMap").show();
        }
    });
}