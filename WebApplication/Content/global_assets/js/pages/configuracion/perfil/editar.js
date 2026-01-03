var Editar = function () {

    var _componentFancytree = function () {
        if (!$().fancytree) {
            console.warn('Warning - fancytree_all.min.js is not loaded.');
            return;
        }

        $(".tree").fancytree({
            checkbox: true,
            selectMode: 3,
            icon: false,
            source: {
                url: $("#listar_menu").val() + "/" + $("#per_cod").val(),
                cache: false,
                complete: function () {
                    $("#nro_menus").val($.ui.fancytree.getTree(".tree").getSelectedNodes().length);
                    var tree = $.ui.fancytree.getTree(".tree");
                    var d = tree.toDict(true);

                    console.log(JSON.stringify(d));

                    $("#menus").val(JSON.stringify(d));
                }
            },
            strings: {
                loading: "Cargando . . . "
            },
            select: function (event, data) {
                var seleccionados = 0;
                var selKeys = $.map(data.tree.getSelectedNodes(), function (node) {
                    seleccionados += 1;
                    return node.key;
                });
                var tree = $.ui.fancytree.getTree(".tree");
                var d = tree.toDict(true);
                $("#menus").val(JSON.stringify(d));
                $("#nro_menus").val(seleccionados);
            },
            loadChildren: function () {
                $("#nro_menus").val($.ui.fancytree.getTree(".tree").getSelectedNodes().length);
                var tree = $.ui.fancytree.getTree(".tree");
                var d = tree.toDict(true);
                $("#menus").val(JSON.stringify(d));
            }
        });


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
                else {
                    error.insertAfter(element);
                }
            },
            rules: {
                per_nom: {
                    required: true,
                    minlength: 5,
                    maxlength: 50,
                    remote: {
                        url: $("#hddn_Valid_Perfil").val(),
                        type: "post",
                        dataType: "json",
                        data: {
                            perfil: function () { return $("#per_cod").val() + '|' + $("#per_nom").val(); }
                        }
                    }
                },
                nro_menus: {
                    min: 1
                }
            },
            messages: {
                per_nom: {
                    required: "El PERFIL es requerido.",
                    minlength: "Favor ingrese un PERFIL válido. (Mín 5 Caracteres)",
                    maxlength: "Favor ingrese un PERFIL válido. (Máx 50 Caracteres)",
                    remote: "El PERFIL ya existe"
                },
                nro_menus: {
                    required: "No se ha seleccionado ninguna opción de MENU.",
                    min: "No se ha seleccionado ninguna opción de MENU."
                }
            }

        });
    };

    return {
        init: function () {
            _componentFancytree();
            _componentValidation();
        }
    };
}();


document.addEventListener('DOMContentLoaded', function () {
    Editar.init();
});