/* ------------------------------------------------------------------------------
* Nombre del clase		    : reporte.js
* Fecha de creación		    : 30-07-2022
* Usuario de creación	    : Iván Villanueva Garrido
* Versión				    : 1.0
* Fecha de modificación	    :
* Usuario de modificación   :
* Motivo de modificación	:
* Objetivo				    : Almacena funciones de formulario de reportes
 * ---------------------------------------------------------------------------- */

// Setup module
// -----------------------------
var Reportes = function () {

    var _componentValidation = function () {
        if (!$().validate) {
            console.warn("Warning - validate.min.js is not loaded.");
            return;
        }

        let validatorInforme1 = $(".form-validate-informe1").validate({
            ignore: "input[type=hidden], .select2-search__field", // ignore hidden fields
            errorClass: "validation-invalid-label",
            successClass: "validation-valid-label",
            validClass: "validation-valid-label",
            highlight: function (element, errorClass) {
                $(element).removeClass(errorClass);
            },
            unhighlight: function (element, errorClass) {
                $(element).removeClass(errorClass);
            },
            errorPlacement: function (error, element) {
                if (element.parents().hasClass("form-check")) {
                    error.appendTo(element.parents(".form-check").parent());
                } else if (element.parents().hasClass("form-group-feedback") ||
                    element.hasClass("select2-hidden-accessible")) {
                    error.appendTo(element.parent());
                } else if (element.parent().is(".uniform-uploader, .uniform-select") ||
                    element.parents().hasClass("input-group")) {
                    error.appendTo(element.parent().parent());
                } else if (element.parents().hasClass("multiselect-native-select")) {
                    error.appendTo(element.parent().parent());
                } else {
                    error.insertAfter(element);
                }
            },
            rules: {
                filtro_reportes: {
                    sinCategoria: true
                }
            },
            messages: {
                filtro_reportes: {
                    sinCategoria: "Completa este campo."
                }
            },
            submitHandler: function (form) {
                //si todos los controles cumplen con las validaciones, se ejecuta este codigo
                const Root = { tipo_elementos: $("#filtro_reportes").val() };
 
                //Call Ajax Method
                $.ajax({
                    url: $("#hddn_get_reporte1").val(),
                    type: "POST",
                    data: JSON.stringify(Root),
                    dataType: "json",
                    contentType: "application/json"
                }).done(function (response) {

                    if (response.status) {

                        const fileContents = response.formModalExportar.fileContents;
                        const contentType = response.formModalExportar.contentType;
                        const fileDownloadName = response.formModalExportar.fileDownloadName;

                        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
                            //console.log('IE');
                            var byteCharacters = atob(fileContents);
                            var byteNumbers = new Array(byteCharacters.length);
                            for (var i = 0; i < byteCharacters.length; i++) {
                                byteNumbers[i] = byteCharacters.charCodeAt(i);
                            }
                            var byteArray = new Uint8Array(byteNumbers);
                            var blob = new Blob([byteArray], { type: "application/vnd.ms-excel" });
                            window.navigator.msSaveOrOpenBlob(blob, fileDownloadName);
                            return;
                        }

                        const link = document.createElement("a");
                        document.body.appendChild(link);
                        link.setAttribute("type", "hidden");
                        link.href = "data:application/vnd.ms-excel;base64," + fileContents;
                        link.download = fileDownloadName;
                        link.click();
                        document.body.removeChild(link);

                    } else {
                        console.log("fnc_Exportar_info1 / Request failed:" + response.responseText);

                    }
                }).fail(function (jqXHR, textStatus) {
                    console.log("fnc_Exportar_info1 / Request failed:" + textStatus);
                }).always(function () {
                    $("#cargando").fadeOut();
                });
                // Fin Call Ajax Method
                return false;
                // fin Metodo
            }
        });

        let validatorInforme2 = $(".form-validate-informe2").validate({
            ignore: "input[type=hidden], .select2-search__field", // ignore hidden fields
            errorClass: "validation-invalid-label",
            successClass: "validation-valid-label",
            validClass: "validation-valid-label",
            highlight: function (element, errorClass) {
                $(element).removeClass(errorClass);
            },
            unhighlight: function (element, errorClass) {
                $(element).removeClass(errorClass);
            },
            errorPlacement: function (error, element) {
                if (element.parents().hasClass("form-check")) {
                    error.appendTo(element.parents(".form-check").parent());
                } else if (element.parents().hasClass("form-group-feedback") ||
                    element.hasClass("select2-hidden-accessible")) {
                    error.appendTo(element.parent());
                } else if (element.parent().is(".uniform-uploader, .uniform-select") ||
                    element.parents().hasClass("input-group")) {
                    error.appendTo(element.parent().parent());
                } else if (element.parents().hasClass("multiselect-native-select")) {
                    error.appendTo(element.parent().parent());
                } else {
                    error.insertAfter(element);
                }
            },
            rules: {
                filtro_tramos: { sinTramo: true },
                filtro_tipo_elemento: { sinTipoElementos: true }
            },
            messages: {
                filtro_tramos: { sinTramo: "Campo obligatorio." },
                filtro_tipo_elemento: { sinTipoElementos: "Campo obligatorio." }
            },
            submitHandler: function (form) {
                //si todos los controles cumplen con las validaciones, se ejecuta este codigo
                const Root = {
                    tramos: $("#filtro_tramos").val(),
                    tipo_elementos: $("#filtro_tipo_elemento").val(),
                    elementos: ["0"]
                };

                console.log(JSON.stringify(Root));

                //Call Ajax Method
                $.ajax({
                    url: $("#hddn_get_reporte2").val(),
                    type: "POST",
                    data: JSON.stringify(Root),
                    dataType: "json",
                    contentType: "application/json"
                }).done(function (response) {

                    if (response.status) {

                        const fileContents = response.formModalExportar.fileContents;
                        const contentType = response.formModalExportar.contentType;
                        const fileDownloadName = response.formModalExportar.fileDownloadName;

                        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
                            //console.log('IE');
                            var byteCharacters = atob(fileContents);
                            var byteNumbers = new Array(byteCharacters.length);
                            for (var i = 0; i < byteCharacters.length; i++) {
                                byteNumbers[i] = byteCharacters.charCodeAt(i);
                            }
                            var byteArray = new Uint8Array(byteNumbers);
                            var blob = new Blob([byteArray], { type: "application/vnd.ms-excel" });
                            window.navigator.msSaveOrOpenBlob(blob, fileDownloadName);
                            return;
                        }

                        const link = document.createElement("a");
                        document.body.appendChild(link);
                        link.setAttribute("type", "hidden");
                        link.href = "data:application/vnd.ms-excel;base64," + fileContents;
                        link.download = fileDownloadName;
                        link.click();
                        document.body.removeChild(link);

                    } else {
                        console.log("fnc_Exportar_info2 / Request failed:" + response.responseText);
                        alert(response.responseText);
                    }
                }).fail(function (jqXHR, textStatus) {
                    console.log("fnc_Exportar_info2 / Request failed:" + textStatus);
                }).always(function () {
                    $("#cargando").fadeOut();
                });
                // Fin Call Ajax Method
                return false;
                // fin Metodo
            }
        });

        jQuery.validator.addMethod("sinCategoria", function (value, element) {

            if (value.length === 0) {
                return false;
            } else {
                return true;
            }
        });

        jQuery.validator.addMethod("sinTramo", function (value, element) {
 
            if (value.length === 0) {
                return false;
            } else {
                return true;
            }
        });

        jQuery.validator.addMethod("sinTipoElementos", function (value, element) {
 
            if (value.length === 0) {
                return false;
            } else {
                return true;
            }
        });
    };

    var _componentReportes = function () {
        $(".collapse").collapse();
        $(".alert").alert();

        $(".collapse").on("shown.bs.collapse",
            function () {
                $(this).parent().addClass("active");
            });

        $(".collapse").on("hidden.bs.collapse",
            function () {
                $(this).parent().removeClass("active");
            });

        //MUESTRA LOADING CUANDO EJECUTA EVENTO AJAX
        $(document).ajaxStart(function () {
            $("#cargando").fadeIn();
        });

        //ESCONDE LOADING CUANDO TERMINA EVENTO AJAX
        $(document).ajaxSuccess(function () {
            $("#cargando").fadeOut();
        });

        //ESCONDE LOADING CUANDO OCURRE ERROR EN EVENTO AJAX
        $(document).ajaxError(function () {
            $("#cargando").fadeOut();
        });

        $(function () {
            $(document).on("click",
                ".alert-close",
                function () {
                    $(this).parent().hide();
                });
        });
    };

    var _componentMultiselect = function () {
        if (!$().multiselect) {
            console.warn("Warning - bootstrap-multiselect.js is not loaded.");
            return;
        }

        $("#filtro_reportes").multiselect({
            enableFiltering: true,
            filterPlaceholder: "Buscar...",
            enableCaseInsensitiveFiltering: true,
            nonSelectedText: "Seleccione Reportes",
            nSelectedText: " Reportes",
            nSelectedAll: " Reportes",
            allSelectedText: "Todos",
            selectAllText: "Seleccionar Todos",
            includeSelectAllOption: true,
            onChange: function (element, checked) {
                $("#reportes").val(JSON.stringify($("#filtro_reportes").val()));
                $("#filtro_reportes-error").remove();
            },
            onSelectAll: function () {
                $("#reportes").val(JSON.stringify($("#filtro_reportes").val()));
                $("#filtro_reportes-error").remove();
            },
            onDeselectAll: function () {
                $("#reportes").val(JSON.stringify($("#filtro_reportes").val()));
                $("#filtro_reportes-error").remove();
            },
            onDropdownHide: function () {
                let reportesArray = $("#filtro_reportes").val();

                console.log(JSON.stringify(reportesArray));
            }
        });

        $("#filtro_tramos").multiselect({
            enableFiltering: true,
            filterPlaceholder: "Buscar...",
            enableCaseInsensitiveFiltering: true,
            nonSelectedText: "Seleccione Tramos",
            nSelectedText: " Tramos",
            nSelectedAll: " Tramos",
            allSelectedText: "Todos",
            selectAllText: "Seleccionar Todos",
            includeSelectAllOption: true,
            onChange: function (element, checked) {
                $("#tramos").val(JSON.stringify($("#filtro_tramos").val()));
                $("#filtro_tramos-error").remove();
            },
            onSelectAll: function () {
                $("#tramos").val(JSON.stringify($("#filtro_tramos").val()));
                $("#filtro_tramos-error").remove();
            },
            onDeselectAll: function () {
                $("#tramos").val(JSON.stringify($("#filtro_tramos").val()));
                $("#filtro_tramos-error").remove();
            },
            onDropdownHide: function () {
                let tramosArray = $("#filtro_tramos").val();

                //console.log(jQuery.isEmptyObject(tramosArray));

                if (!jQuery.isEmptyObject(tramosArray)) {
                    fnc_devolver_tipo_elementos();
                }
            }
        });

        $("#filtro_tipo_elemento").multiselect({
            enableFiltering: true,
            filterPlaceholder: "Buscar...",
            enableCaseInsensitiveFiltering: true,
            nonSelectedText: "Seleccione Tipo Elementos",
            nSelectedText: " Elementos",
            nSelectedAll: " Elementos",
            allSelectedText: "Todos",
            selectAllText: "Seleccionar Todos",
            includeSelectAllOption: true,
            onChange: function (element, checked) {
                $("#tipo_elemento").val(JSON.stringify($("#filtro_tipo_elemento").val()));
                $("#filtro_tipo_elemento-error").remove();
            },
            onSelectAll: function () {
                $("#tipo_elemento").val(JSON.stringify($("#filtro_tipo_elemento").val()));
                $("#filtro_tipo_elemento-error").remove();
            },
            onDeselectAll: function () {
                $("#tipo_elemento").val(JSON.stringify($("#filtro_tipo_elemento").val()));
                $("#filtro_tipo_elemento-error").remove();
            },
            onDropdownHide: function () {
                /*                console.log($("#filtro_tipo_elemento").val());*/
            }
        });

    };

    var _componentCargaFiltros = function () {

        //Call Ajax Method
        $.ajax({
            url: $("#hddn_get_data").val(),
            type: "POST",
            dataType: "json",
            contentType: "application/json"
        }).done(function (response) {

            /*        console.log(JSON.stringify(response)) */

            // 1) -  FILTRO REPORTES
            $("#filtro_reportes").multiselect("enable");
            $("option", $("#filtro_reportes")).remove();

            var reportes = response.List;
            reportes = reportes.map(
                function (reportes) {
                    $("#filtro_reportes")
                        .append('<option value="' + reportes.Value + '">' + reportes.Text + "</option>");
                });

            $("#filtro_reportes").multiselect("rebuild");

            //2) - FILTRO TRAMOS
            $("#filtro_tramos").multiselect("enable");
            $("option", $("#filtro_tramos")).remove();

            var tramos = response.AllTramosList;
            tramos = tramos.map(
                function (tramos) {
                    $("#filtro_tramos")
                        .append('<option value="' + tramos.Value + '">' + tramos.Text + "</option>");
                });

            $("#filtro_tramos").multiselect("rebuild");


        }).fail(function (jqXHR, textStatus) {
            console.log("Request failed: " + textStatus);
        }).always(function () {
            $("#cargando").fadeOut();
        });
        // Fin Call Ajax Method
    };

    return {
        init: function () {
            _componentReportes();
            _componentMultiselect();
            //_componentCargaFiltros();
            _componentValidation();
        }
    };
}();

document.addEventListener("DOMContentLoaded",
    function () {
        Reportes.init();
    });

// Functions module
// -----------------------------
function fnc_devolver_tipo_elementos() {

    const Root = {
        switch_case: "TIPO",
        tramos: $("#filtro_tramos").val(),
        tipo_elementos: [],
        elementos: []
    };

    //Call Ajax Method
    $.ajax({
        url: $("#hddn_get_data_elementos").val(),
        type: "POST",
        data: JSON.stringify(Root),
        dataType: "json",
        contentType: "application/json"

    }).done(function (response) {
        $("#filtro_tipo_elemento").html("");

        console.log(JSON.stringify(response));

        if (response.status == true) {
            // 1) -  FILTRO REPORTES
            $("#filtro_tipo_elemento").multiselect("enable");
            $("option", $("#filtro_tipo_elemento")).remove();

            var ejecutivos = response.List;

            var html = "";
            var group = "";

            // data has to be in this format for the following loop:
            $.each(ejecutivos,
                function (optGroup, elements) {

                    if (group != elements.Group.Name) {
                        html += '<optgroup label="' + elements.Group.Name + '">';
                        group = elements.Group.Name;

                        $.each(ejecutivos,
                            function (optGroup, elements) {

                                if (group == elements.Group.Name) {

                                    html += '<option value="' + elements.Value + '">' + elements.Text + "</option>";
                                }

                            });
                        html += "</optgroup>";
                    }
                });

            $("#filtro_tipo_elemento").append(html);
            $("#filtro_tipo_elemento").multiselect("rebuild");
        }
        else {
            console.log('falso');
            $("#filtro_tipo_elemento").multiselect("enable");
            $("option", $("#filtro_tipo_elemento")).remove();

            let swalInit = swal.mixin({
                buttonsStyling: false,
                confirmButtonClass: 'btn btn-primary',
                cancelButtonClass: 'btn btn-light'
            });

            swalInit.fire({
                title: 'INFORMES',
                html: response.message,
                type: 'error',
                showCancelButton: false,
                confirmButtonText: '<i class="icon-checkmark2 mr-1"></i>Aceptar',
                confirmButtonClass: 'btn btn-primary',
                buttonsStyling: false
            });
        }

    }).fail(function (jqXHR, textStatus) {
        console.log("fnc_devolver_tipo_elementos / Request failed: " + textStatus);
    }).always(function () {
        $("#cargando").fadeOut();
    });
    // Fin Call Ajax Method
}

function fnc_Exportar_info1() {
    $("#form_informe1").submit();
}
function fnc_Exportar_info2() {
    $("#form_informe2").submit();
}
function fnc_Exportar_info3() {
    //si todos los controles cumplen con las validaciones, se ejecuta este codigo
    const Root = { reportes: null };

    //Call Ajax Method
    $.ajax({
        url: $("#hddn_get_reporte3").val(),
        type: "POST",
        data: JSON.stringify(Root),
        dataType: "json",
        contentType: "application/json"
    }).done(function (response) {

        if (response.status) {

            const fileContents = response.formModalExportar.fileContents;
            const contentType = response.formModalExportar.contentType;
            const fileDownloadName = response.formModalExportar.fileDownloadName;

            if (window.navigator && window.navigator.msSaveOrOpenBlob) {
                //console.log('IE');
                var byteCharacters = atob(fileContents);
                var byteNumbers = new Array(byteCharacters.length);
                for (var i = 0; i < byteCharacters.length; i++) {
                    byteNumbers[i] = byteCharacters.charCodeAt(i);
                }
                var byteArray = new Uint8Array(byteNumbers);
                var blob = new Blob([byteArray], { type: "application/vnd.ms-excel" });
                window.navigator.msSaveOrOpenBlob(blob, fileDownloadName);
                return;
            }

            const link = document.createElement("a");
            document.body.appendChild(link);
            link.setAttribute("type", "hidden");
            link.href = "data:application/vnd.ms-excel;base64," + fileContents;
            link.download = fileDownloadName;
            link.click();
            document.body.removeChild(link);

        } else {
            console.log("fnc_Exportar_info3 / Request failed:" + response.responseText);

            let swalInit = swal.mixin({
                buttonsStyling: false,
                confirmButtonClass: 'btn btn-primary',
                cancelButtonClass: 'btn btn-light'
            });

            swalInit.fire({
                title: 'INFORMES',
                html: response.responseText,
                type: 'error',
                showCancelButton: false,
                confirmButtonText: '<i class="icon-checkmark2 mr-1"></i>Aceptar',
                confirmButtonClass: 'btn btn-primary',
                buttonsStyling: false
            });
        }
    }).fail(function (jqXHR, textStatus) {
        console.log("fnc_Exportar_info3 / Request failed:" + textStatus);
    }).always(function () {
        $("#cargando").fadeOut();
    });
    // Fin Call Ajax Method
    return false;
    // fin Metodo
}