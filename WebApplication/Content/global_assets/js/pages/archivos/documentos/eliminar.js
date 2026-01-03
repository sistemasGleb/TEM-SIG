var Eliminar = function () {

    return {
        init: function () {
            //_componentValidation();
        }
    }
}();

document.addEventListener('DOMContentLoaded', function () {
    Eliminar.init();
});

function fnc_ConfirmarEliminar(pGuid) {
    $('#cargando').fadeIn();

    var data = { pGuid: pGuid };

    //Call Ajax Method
    $.ajax({
        url: $("#hiddenEliminarArchivo").val(),
        type: "GET",
        data: data,
        dataType: "json",
        contentType: "application/json"
    }).done(function (response) {

        console.log(JSON.stringify(response));

        if (response.status == "success") {
            let swalInit = swal.mixin({
                buttonsStyling: false,
                confirmButtonClass: 'btn btn-primary',
                cancelButtonClass: 'btn btn-light'
            });

            swalInit.fire({
                title: 'Eliminar Documento',
                html: response.exception,
                type: 'success',
                showCancelButton: false,
                confirmButtonText: '<i class="icon-checkmark2 mr-1"></i>Aceptar',
                confirmButtonClass: 'btn btn-primary',
                buttonsStyling: false,
                allowOutsideClick: false,
                hideClass: {
                    popup: 'animate__animated animate__fadeOutUp'
                }
            }).then(function (result) {
                if (result.value) {
                    fnc_Listar();
                }
            });
        }
        else {
            let swalInit = swal.mixin({
                buttonsStyling: false,
                confirmButtonClass: 'btn btn-primary',
                cancelButtonClass: 'btn btn-light'
            });

            swalInit.fire({
                title: 'Eliminar Documento',
                html: response.exception,
                type: 'error',
                showCancelButton: false,
                confirmButtonText: '<i class="icon-checkmark2 mr-1"></i>Aceptar',
                confirmButtonClass: 'btn btn-primary',
                buttonsStyling: false,
                allowOutsideClick: false,
            });
        }
    }).fail(function (jqXHR, textStatus) {
        console.log("fnc_ConfirmarEliminar / Request failed : " + textStatus);
    }).always(function() {
        $('#cargando').fadeOut();
    });
    // /.Call Ajax Method
}