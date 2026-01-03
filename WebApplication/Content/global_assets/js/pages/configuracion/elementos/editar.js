var Editar = function () {

    var _componentBootstrapSwitch = function () {
        if (!$().bootstrapSwitch) {
            console.warn('Warning - switch.min.js is not loaded.');
            return;
        }

        $('.form-check-input-switch').bootstrapSwitch({
            onText: 'Sí',
            onColor: 'success',
            offText: 'No',
            offColor: 'danger',
            size: 'large'
        });

        $('.form-check-input-switch').bootstrapSwitch();
    };
    var _componentValidation = function () {
        if (!$().validate) {
            console.warn('Warning - validate.min.js is not loaded.');
            return;
        }

        var validator = $('.form-validate').validate({
            ignore: 'input[type=hidden], .select2-search__field', // ignore hidden fields
            errorClass: 'validation-invalid-label',
            successClass: 'validation-valid-label',
            validClass: 'validation-valid-label',
            highlight: function (element, errorClass) {
                $(element).removeClass(errorClass);
            },
            unhighlight: function (element, errorClass) {
                $(element).removeClass(errorClass);
            },
            errorPlacement: function (error, element) {
                if (element.parents().hasClass('form-check')) {
                    error.appendTo(element.parents('.form-check').parent());
                }
                else if (element.parents().hasClass('form-group-feedback') || element.hasClass('select2-hidden-accessible')) {
                    error.appendTo(element.parent());
                }
                else if (element.parent().is('.uniform-uploader, .uniform-select') || element.parents().hasClass('input-group')) {
                    error.appendTo(element.parent().parent());
                }
                else if (element.parents().hasClass('multiselect-native-select')) {
                    error.appendTo(element.parent().parent());
                }
                else {
                    error.insertAfter(element);
                }
            },
            rules: {
                Param_id: { required: true },
                Param_ele_nom: { required: true },
                Param_tra_id: { required: true, min: 1 },
                Param_lad: { required: true, min: 1 },
                Param_dm_ini: {
                    required: true,
                    min: 0.01,
                    normalizer: function (value) {
                        return this.value.replace(/\,/g, '.');
                    }
                },
                Param_dm_fin: {
                    min: 0.01,
                    normalizer: function (value) {
                        return this.value.replace(/\,/g, '.');
                    }
                },
                Param_crd_lat_ini: {
                    required: true,
                    latCoord: true,
                    normalizer: function (value) {
                        return this.value.replace(/\,/g, '.');
                    }
                },
                Param_crd_lon_ini: {
                    required: true,
                    longCoord: true,
                    normalizer: function (value) {
                        return this.value.replace(/\,/g, '.');
                    }
                },
                Param_crd_lat_fin: {
                    latCoord: true,
                    normalizer: function (value) {
                        return this.value.replace(/\,/g, '.');
                    }
                },
                Param_crd_lon_fin: {
                    longCoord: true,
                    normalizer: function (value) {
                        return this.value.replace(/\,/g, '.');
                    }
                },
                Param_Mac: { required: true, min: 1 },
            },
            messages: {
                Param_id: { required: "Requerido." },
                Param_ele_nom: { required: "Requerido." },
                Param_tra_id: { required: "Requerido", min: "Requerido." },
                Param_lad: { required: "Requerido", min: "Requerido." },
                Param_dm_ini: { required: "Requerido.", min: "DM Inválido." },
                Param_dm_fin: { min: "DM Inválido" },
                Param_crd_lat_ini: { required: "Requerido.", latCoord: "Formato incorrecto." },
                Param_crd_lon_ini: { required: "Requerido.", longCoord: "Formato incorrecto." },
                Param_crd_lat_fin: { required: "Requerido.", latCoord: "Formato incorrecto." },
                Param_crd_lon_fin: { required: "Requerido.", longCoord: "Formato incorrecto." },
                Param_Mac: { required: "Requerido.", min: "Requerido." }
            }
        });

        jQuery.validator.addMethod("latCoord", function (value, element) {
        
                return this.optional(element) ||
                    value.length >= 4 && /^(?=.)-?((8[0-5]?)|([0-7]?[0-9]))?(?:\.[0-9]{1,20})?$/.test(value);
            });

        jQuery.validator.addMethod("longCoord", function (value, element) {
    
                return this.optional(element) ||
                    value.length >= 4 &&
                    /^(?=.)-?((0?[8-9][0-9])|180|([0-1]?[0-7]?[0-9]))?(?:\.[0-9]{1,20})?$/.test(value);
            });
    };

    var _componentSelect2 = function () {
        if (!$().select2) {
            console.warn('Warning - select2.min.js is not loaded.');
            return;
        }

        $('.select2').select2();

        $('#Param_tra_id').on('select2:select', function (e) {
            $("#Param_tra_id-error").remove();
        });

    };

    var _componentDaterangeSingle = function () {
        if (!$().daterangepicker) {
            console.warn('Warning - daterangepicker.js is not loaded.');
            return;
        }

        $('.daterange-single-fin').daterangepicker({
            singleDatePicker: true,
            autoUpdateInput: fnc_IsNullOrEmpty($("#Param_fec_ins").val()),
            locale: {
                format: 'DD/MM/YYYY',
                daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                firstDay: 1
            },
            startDate: _startDate($("#Param_fec_ins").val())
        }).on('apply.daterangepicker', function (ev, picker) {
            $("#Param_fec_ins").val(picker.endDate.format('DD/MM/YYYY'));

            $(this).val(picker.endDate.format('DD/MM/YYYY'));

           console.log($("#Param_fec_ins").val()) ;
            $("#fecha_inicio-error").remove();
        });
    };

    var _componentForm = function () {

        $("#Param_dm_ini").keyup(function () {
            var $this = $(this);
            $this.val($this.val().replace(/[^\d,]/g, ''));
        });

        $("#Param_dm_fin").keyup(function () {
            var $this = $(this);
            $this.val($this.val().replace(/[^\d,]/g, ''));
        });

        $('#rangeDemoClear').on('click', function (e) {
            $('#fecha_fin').val('').trigger('change');
            $('#Param_fec_ins').val('');
        });

    };

    var _componentDatatable = function () {
        if (!$().DataTable) {
            console.warn('Warning - datatables.min.js is not loaded.');
            return;
        }

        $.extend($.fn.dataTable.defaults, {
            autoWidth: false,
            columnDefs: [{
                orderable: false,
            }],
            dom: '<"datatable-header"fBl><"datatable-scroll"t><"datatable-footer"ip>',
            language: {
                info: "Mostrando página _PAGE_ de _PAGES_",
                zeroRecords: "No se encontraron registros.",
                infoEmpty: "No se encontraron registros.",
                emptyTable: "No hay información para los filtros indicados.",
                infoFiltered: "(Filtrando entre _MAX_ registros)",
                search: '_INPUT_',
                searchPlaceholder: 'Buscar...',
                lengthMenu: '<span>Mostrar:</span> _MENU_',
                paginate: { 'first': 'Primero', 'last': 'Ultimo', 'next': $('html').attr('dir') == 'rtl' ? '&larr;' : '&rarr;', 'previous': $('html').attr('dir') == 'rtl' ? '&rarr;' : '&larr;' }
            }
        });

        $('.table').DataTable({
            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "Todos"]],
            order: [[0, 'asc']],
            buttons: [
                {
                    text: 'Agregar<i class="icon-googleplus5 ml-2 disabled "></i>',
                    className: 'btn bg-teal-400',
                    action: function () {
                        console.log('add new image');

                        let idMarker = $('#Param_id').val();
                        let typeMarker = $('#Param_tip_ele').val();
                        let nameMarker = $('#Param_ele_nom').val();

                        window.location.href = $("#hiddenUploaImagedFile").val() + "?id_tipo_elemento=" + typeMarker + "&id_elemento=" + idMarker + "&nombre_elemento=" + nameMarker ;
                    }
                }
            ]
        });

        $("div.datatable-header").append('<div class="text-center"><h4>Galeria De Imágenes</h4></div>');
    };


    return {
        init: function () {
            _componentDatatable();
            _componentBootstrapSwitch();
            _componentSelect2();
            _componentValidation();
            _componentDaterangeSingle();
            _componentForm();
        }
    };
}();


document.addEventListener('DOMContentLoaded', function () {
    Editar.init();
});


function _startDate(str) {
    var dt = new Date();

    if (!fnc_IsNullOrEmpty(str)) {
        $("#Param_fec_ins").val(dt);
        return dt;
    }

    return $("#Param_fec_ins").val();

}

// - Indicates whether the specified string is null or an empty string ("").
function fnc_IsNullOrEmpty(str) {

    if (typeof str == 'undefined' || !str || str.length === 0 || str === "" || !/[^\s]/.test(str) || /^\s*$/.test(str) || str.replace(/\s/g, "") === "")
        return false;
    else
        return true;
}

function fnc_validar_formulario() {
 
            $("#form_editar_elemento").submit();
}

function fnc_VerElementoImagen(path, img) {

    // 1)- path Imagen
    let sFilepath = path.concat(img); // '/content/img/elementos/0022/00_fosoycontrafoso.png'
    $('#filePath').attr('src', sFilepath);

    $('#img_filePath_name').text(img);

    $('#staticBackdropImagePreview').appendTo("body").modal('show');
}
function fnc_EliminarElementoImagen(idTipo,idElemento,idImage)
{
    let usuario = '';

    bootbox.confirm({
        title: 'Eliminar Imagen',
        message: '¿Está seguro de que desea eliminar la imagen de la galeria?',
        html: '',
        buttons: {
            confirm: {
                label: 'Sí, Eliminar imagen',
                className: 'btn-danger'
            },
            cancel: {
                label: 'No, Mantener imagen',
                className: 'btn-light'
            }
        },
        callback: function (result) {
            if (result)
                window.location.href = $("#elmento_img_eliminar").val() +   "?id_tipo_elemento=" + idTipo + "&id_elemento=" + idElemento + "&id=" + idImage;
        }
    });

}
function fnc_volver_elementos() {

    let idTipoElemento = $('#Param_tip_ele').val();
 
    window.location.href = $("#hiddenlistarElemento").val() + '?id_tipo_elemento=' + idTipoElemento;
}

