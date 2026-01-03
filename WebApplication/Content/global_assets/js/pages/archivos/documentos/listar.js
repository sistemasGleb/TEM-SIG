var Listar = function () {

    var _listarDocumentos = function () {

        fnc_Listar();
    };

    var _componentDatatable = function () {
        if (!$().DataTable) {
            console.warn('Warning - datatables.min.js is not loaded.');
            return;
        }

        $.extend($.fn.dataTable.defaults, {
            autoWidth: false,
            searching: true,
            columnDefs: [{
                orderable: false,
            }],
            dom: '<"datatable-header"fB><"datatable-scroll"t><"datatable-footer">',
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

        $.extend($.fn.dataTable.ext.type.order, {
            "fechahora-pre": function (name) {
                return new Date($(name)[0].children[0].innerText);
            },
            "fechahora-asc": function (a, b) {
                return a - b;
            },
            "fechahora-desc": function (a, b) {
                return b - a;
            }
        });

        $.extend($.fn.dataTable.ext.type.order, {
            "ordentexto-pre": function (name) {
                //console.log($(name).find(".order-val")[0].innerHTML);
                return $(name).find(".order-val")[0].innerHTML;
            }
        });
    };

    return {
        init: function () {
            _componentDatatable();
            _listarDocumentos();
        }
    }
}();

document.addEventListener('DOMContentLoaded', function () {
    Listar.init();
});

function fnc_Listar() {
    var _flag = false;
    $("#card_PortabilidadDocsMessage").text('');
    $("#card_PortabilidadDocsAlert").removeClass().addClass("alert alert-danger alert-styled-left alert-dismissible d-none");

    //Call Ajax Method listar_conectados
    $.ajax({
        url: $("#listar_documentos").val(),
        type: "GET",
        contentType: "application/json"
    }).done(function (response) {

        //console.log(JSON.stringify(response));

        if (response.status == "success") {

            $('#table_JsonDocumentos').DataTable().destroy();

            var table_documentos = $('#table_JsonDocumentos').DataTable({
                paging: false,
                retrieve: true,
                ordering: true,
                data: response.list,
                columns: [
                    { data: "doc_cat_nom", visible: false },
                    { data: null, type: 'fechahora', className: 'text-center' },
                    { data: null, type: 'ordentexto' },
                    { data: null, type: 'ordentexto' },
                    { data: "doc_usr_mod_nom", className: 'text-lowercase text-capitalize' },
                    { data: null, orderable: false, className: 'text-center' }
                ],
                drawCallback: function (settings) {
                    var api = this.api();
                    var rows = api.rows({ page: 'current' }).nodes();
                    var last = null;

                    api.column(0, { page: 'current' }).data().each(function (group, i) {
                        if (last !== group) {
                            $(rows).eq(i).before(
                                '<tr class="table-active table-border-double"><td colspan="5" class="font-weight-semibold"><i class="icon-folder-upload mr-3 icon-1x"></i> ' + group + '</td></tr>'
                            );
                            last = group;
                        }
                    });
                },
                columnDefs: [
                    {
                        targets: 1, render: function (data) {
                            return fnc_AddColFecha(data);
                        }
                    },
                    {
                        targets: 2, render: function (data) {
                            return fnc_AddColTitulo(data);
                        }
                    },
                    {
                        targets: 3, render: function (data) {
                            return fnc_AddColDescripcion(data);
                        }
                    },
                    {
                        targets: 5, render: function (data) {
                            return fnc_AddOptionButton(data);
                        }
                    }
                ]
            });

            table_documentos.buttons().remove();

   /*         if ($("#administra_archivos").val() == "true") {*/

                table_documentos.button().add(0, {
                    text: '<i class="icon-list mr-2"></i>Biblioteca',
                    className: 'btn bg-primary-400 mr-2',
                    titleAttr: 'Administra Biblioteca',
                    action: function () {
                        window.location.href = $("#maestro_categorias").val();
                    }
                });

                table_documentos.button().add(1, {
                    text: '<i class="icon-file-upload2 mr-2"></i>Subir archivo',
                    className: 'btn bg-teal-400',
                    titleAttr: 'Carga un Nuevo Documento',
                    action: function () {
                        window.location.href = $("#hiddenUpload").val();
                    }
                });
  /*          }*/

            _flag = true;
        }
        else {
            $("#card_PortabilidadDocsMessage").text(response.exception);
            $("#card_PortabilidadDocsAlert").removeClass("d-none").addClass("d-block");
        }
    }).fail(function (response) {
        $("#card_PortabilidadDocsMessage").text(response.exception);
        $("#card_PortabilidadDocsAlert").removeClass("d-none").addClass("d-block");
    }).always(function () {

        if (_flag) { $("#table_Responsive").removeClass("d-none").addClass("d-block"); }
        else { $("#table_Responsive").removeClass("d-block").addClass("d-none"); }

    });
    // Fin Call Ajax Method
}

// tabla
function fnc_AddColFecha(data) {

    let linea = '';
    linea += '        <div class="font-size-sm text-muted line-height-1">' + data.doc_usr_cre_glosa + '<span style="display:none;">' + data.doc_usr_cre_fec + '</span></div>';

    return linea;
}
function fnc_AddColTitulo(data) {

    let linea = '';
    linea += '        <div class="d-flex align-items-center">';
    linea += '            <div class="mr-3">';

    // Icono segun su extension
    linea += fnc_tbl_tr_icon(data.doc_extension);

    linea += '                </a>';
    linea += '            </div>';
    linea += '            <div>';
    linea += '                <a href="#" onclick=" fnc_DescargarDocumento(\'' + data.doc_guid + '\');" class="text-default font-weight-semibold letter-icon-title order-val">' + data.doc_titulo + '</a>';
    linea += '                <div class="text-muted align-middle font-size-sm" title="Compartido por : ' + data.doc_usr_cre_nom + '"><i class="icon-file-upload" style="font-size: 0.7rem;"></i> ' + data.doc_usr_cre_nom + '</div>';
    linea += '            </div>';
    linea += '        </div>';

    return linea;
}
function fnc_AddColDescripcion(data) {
    let linea = '';
    linea += '        <a href="#" onclick=" fnc_DescargarDocumento(\'' + data.doc_guid + '\');" class="text-default">';
    linea += '            <div class="font-weight-semibold order-val" title="' + data.doc_nombre + '">' + data.doc_descripcion + '</div>';
    linea += '            <span class="text-muted">' + data.doc_nombre + '</span>';
    linea += '        </a>';
    return linea;
}
function fnc_AddOptionButton(data) {

    let linea = '';
    linea += '<div class="list-icons">';

    if (data.doc_file_exists) {
        linea += '  <a href="#" class="icon-file-download2 text-primary" title="Descargar" onclick=" fnc_DescargarDocumento(\'' + data.doc_guid + '\');"></a>';
    } else {
        linea += '  <a href="#" class="icon-file-minus2 text-danger-600 text-muted" title="El archivo no existe en el repositorio."></a>';
    }

    if (data.doc_file_delete) {
        linea += '  <a href="#" class="icon-trash text-danger-600 pl-2" title="Eliminar" onclick=" fnc_EliminarDocumento(\'' + data.doc_guid + '\');"></a>';
    }

    linea += '</div>';

    return linea;
}

function fnc_tbl_tr_icon(data) {

    let sb = '';    //string will be appended later

    if (data === 'DOC' || data === 'DOCX') {
        sb += '	<i class="icon-file-word mr-2 icon-2x" style="color:blue;"></i>';
    }
    else if (data === 'PDF') {
        sb += '	<i class="icon-file-pdf mr-2 icon-2x" style="color:red;"></i>';
    }
    else if (data === 'PPTX' || data === 'PPT') {
        sb += '	<i class="icon-file-presentation mr-2 icon-2x" style="color:orange;"></i>';
    }
    else if (data === 'XLSX' || data === 'XLS') {
        sb += '	<i class="icon-file-excel mr-2 icon-2x" style="color:green;"></i>';
    }
    else if (data === 'JPG' || data === 'PNG') {
        sb += '	<i class="icon-file-picture mr-2 icon-2x"></i>';
    }
    else if (data === 'ZIP' || data === 'RAR') {
        sb += '	<i class="icon-file-zip mr-2 icon-2x"></i>';
    }
    else if (data === 'TXT') {
        sb += '	<i class="icon-file-text mr-2 icon-2x"></i>';
    }
    else {
        sb += '	<i class="icon-file-empty mr-2 icon-2x"></i>';
    }

    return sb;
}

// Subir 


function fnc_SubirDocumento() {

    // Ocultamos loading
    fncSubmitbutton();

    // Mensajes
    $("#alert_success_message").text('');
    $("#row_alert_success").css("display", "none");

    $("#alert_danger_message").text('');
    $("#row_alert_danger").css("display", "none");

    // Limpiamos inputs
    $("#ddlCategorias-error").remove();
    $("#txtTitulo-error").remove();
    $("#txtDescripcion-error").remove();
    $("#fileArchivoCarga-error").remove();

    //Reestablecemos los input del fromulario
    $("#ddlCategorias").val(0).trigger('change');

    $("#formulario")[0].reset();

    // Mostramos el formulario
    $("#row_formulario").css("display", "block");

    $("#row_footer").removeClass().addClass("modal-footer d-flex justify-content-end mb-1");
    $("#row_footer").css("display", "block");

    $('#modalSubirArchivo').appendTo("body").modal('show');
}

function fncLoadingbutton() {

    $("#btn-example-file-close").prop('disabled', true);
    $("#btn-example-file-reset").prop('disabled', true);

    $("#btn-example-file-submit").removeClass("btn btn-success btn-sm").addClass("btn btn-light btn-loading btn-sm");
    $("#btn-example-file-submit").html("<i class='icon-spinner2 spinner mr-2'></i> Cargando...");
    $("#btn-example-file-submit").prop('disabled', true);
    return false;
}
function fncSubmitbutton() {
    $("#btn-example-file-close").prop('disabled', false);
    $("#btn-example-file-reset").prop('disabled', false);

    $("#btn-example-file-submit").removeClass("btn btn-light btn-loading").addClass("btn btn-success");
    $("#btn-example-file-submit").html("Subir Documento <i class='icon-cloud-upload ml-1'></i>");
    $("#btn-example-file-submit").prop('disabled', false);
    return false;
}

// Eliminar
function fnc_EliminarDocumento(guid, mombre) {

    let swalInit = swal.mixin({
        buttonsStyling: false,
        confirmButtonClass: 'btn btn-primary',
        cancelButtonClass: 'btn btn-light'
    });

    swalInit.fire({
        title: 'Eliminar Documento',
        html: '¿Está seguro de que desea eliminar este documento?',
        type: 'question',
        showCancelButton: true,
        confirmButtonText: '<i class="icon-checkmark2 mr-1"></i>Aceptar',
        cancelButtonText: '<i class="icon-cross3 mr-1"></i>Cancelar',
        confirmButtonClass: 'btn btn-primary',
        cancelButtonClass: 'btn btn-light',
        buttonsStyling: false
    }).then(function (result) {
        if (result.value) {
            fnc_ConfirmarEliminar(guid);
        }
    });

}

// Descargar
function fnc_DescargarDocumento(fileGuid, fileNombre) {

    var url = $("#hiddenDownload").val() + '/?fileGuid=' + fileGuid;
    window.location = url;
}