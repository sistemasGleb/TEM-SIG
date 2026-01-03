/* ------------------------------------------------------------------------------
 *
 *  # Bootstrap multiple file uploader
 *
 *  Demo JS code for uploader_bootstrap.html page
 *
 * ---------------------------------------------------------------------------- */


// Setup module
// ------------------------------

var FileUpload = function() {


    //
    // Setup module components
    //

    // Bootstrap file upload
    var _componentFileUpload = function() {
        if (!$().fileinput) {
            console.warn('Warning - fileinput.min.js is not loaded.');
            return;
        }

        //
        // Define variables
        //

        // Modal template
        var modalTemplate = '<div class="modal-dialog modal-lg" role="document">\n' +
            '  <div class="modal-content">\n' +
            '    <div class="modal-header align-items-center">\n' +
            '      <h6 class="modal-title">{heading} <small><span class="kv-zoom-title"></span></small></h6>\n' +
            '      <div class="kv-zoom-actions btn-group">{toggleheader}{fullscreen}{borderless}{close}</div>\n' +
            '    </div>\n' +
            '    <div class="modal-body">\n' +
            '      <div class="floating-buttons btn-group"></div>\n' +
            '      <div class="kv-zoom-body file-zoom-content"></div>\n' + '{prev} {next}\n' +
            '    </div>\n' +
            '  </div>\n' +
            '</div>\n';

        // Buttons inside zoom modal
        var previewZoomButtonClasses = {
            toggleheader: 'btn btn-light btn-icon btn-header-toggle btn-sm',
            fullscreen: 'btn btn-light btn-icon btn-sm',
            borderless: 'btn btn-light btn-icon btn-sm',
            close: 'btn btn-light btn-icon btn-sm'
        };

        // Icons inside zoom modal classes
        var previewZoomButtonIcons = {
            prev: '<i class="icon-arrow-left32"></i>',
            next: '<i class="icon-arrow-right32"></i>',
            toggleheader: '<i class="icon-menu-open"></i>',
            fullscreen: '<i class="icon-screen-full"></i>',
            borderless: '<i class="icon-alignment-unalign"></i>',
            close: '<i class="icon-cross2 font-size-base"></i>'
        };

        // File actions
        var fileActionSettings = {
            zoomClass: '',
            zoomIcon: '<i class="icon-zoomin3"></i>',
            dragClass: 'p-2',
            dragIcon: '<i class="icon-three-bars"></i>',
            removeClass: '',
            removeErrorClass: 'text-danger',
            removeIcon: '<i class="icon-bin"></i>',
            indicatorNew: '<i class="icon-file-plus text-success"></i>',
            indicatorSuccess: '<i class="icon-checkmark3 file-icon-large text-success"></i>',
            indicatorError: '<i class="icon-cross2 text-danger"></i>',
            indicatorLoading: '<i class="icon-spinner2 spinner text-muted"></i>'
        };


        ////
        // Basic example
        //

        $('.file-input').fileinput({
            fileSingle: 'archivo',
            filePlural: 'archivos',
            browseLabel: 'Examinar &hellip;',
            removeLabel: 'Quitar',
            removeTitle: 'Quitar archivos seleccionados',
            cancelLabel: 'Cancelar',
            cancelTitle: 'Abortar la subida en curso',
            pauseLabel: 'Pause',
            pauseTitle: 'Pause ongoing upload',
            uploadLabel: 'Subir archivo',
            uploadTitle: 'Subir archivos seleccionados',
            msgNo: 'No',
            msgNoFilesSelected: 'No hay archivos seleccionados',
            msgPaused: 'Paused',
            msgCancelled: 'Cancelado',
            msgPlaceholder: 'Seleccionar {files} ...',
            msgZoomModalHeading: 'Vista previa detallada',
            msgFileRequired: 'Debes seleccionar un archivo para subir.',
            msgSizeTooSmall: 'El archivo "{name}" (<b>{size} KB</b>) es demasiado pequeño y debe ser mayor de <b>{minSize} KB</b>.',
            msgSizeTooLarge: 'El archivo "{name}" (<b>{size} KB</b>) excede el tamaño máximo permitido de <b>{maxSize} KB</b>.',
            msgFilesTooLess: 'Debe seleccionar al menos <b>{n}</b> {files} a cargar.',
            msgFilesTooMany: 'El número de archivos seleccionados a cargar <b>({n})</b> excede el límite máximo permitido de <b>{m}</b>.',
            msgTotalFilesTooMany: 'You can upload a maximum of <b>{m}</b> files (<b>{n}</b> files detected).',
            msgFileNotFound: 'Archivo "{name}" no encontrado.',
            msgFileSecured: 'No es posible acceder al archivo "{name}" porque está siendo usado por otra aplicación o no tiene permisos de lectura.',
            msgFileNotReadable: 'No es posible acceder al archivo "{name}".',
            msgFilePreviewAborted: 'Previsualización del archivo "{name}" cancelada.',
            msgFilePreviewError: 'Ocurrió un error mientras se leía el archivo "{name}".',
            msgInvalidFileName: 'Caracteres no válidos o no soportados en el nombre del archivo "{name}".',
            msgInvalidFileType: 'Tipo de archivo no válido para "{name}". Sólo se permiten archivos de tipo "{types}".',
            msgInvalidFileExtension: 'Extensión de archivo no válida para "{name}". Sólo se permiten archivos "{extensions}".',
            msgFileTypes: {
                'image': 'image',
                'html': 'HTML',
                'text': 'text',
                'video': 'video',
                'audio': 'audio',
                'flash': 'flash',
                'pdf': 'PDF',
                'object': 'object'
            },
            msgUploadAborted: 'La carga de archivos se ha cancelado',
            msgUploadThreshold: 'Procesando &hellip;',
            msgUploadBegin: 'Inicializando &hellip;',
            msgUploadEnd: 'Hecho',
            msgUploadResume: 'Resuming upload &hellip;',
            msgUploadEmpty: 'No existen datos válidos para el envío.',
            msgUploadError: 'Upload Error',
            msgDeleteError: 'Delete Error',
            msgProgressError: 'Error',
            msgValidationError: 'Error de validación',
            msgLoading: 'Subiendo archivo {index} de {files} &hellip;',
            msgProgress: 'Subiendo archivo {index} de {files} - {name} - {percent}% completado.',
            msgSelected: '{n} {files} seleccionado(s)',
            msgFoldersNotAllowed: 'Arrastre y suelte únicamente archivos. Omitida(s) {n} carpeta(s).',
            msgImageWidthSmall: 'El ancho de la imagen "{name}" debe ser de al menos {size} px.',
            msgImageHeightSmall: 'La altura de la imagen "{name}" debe ser de al menos {size} px.',
            msgImageWidthLarge: 'El ancho de la imagen "{name}" no puede exceder de {size} px.',
            msgImageHeightLarge: 'La altura de la imagen "{name}" no puede exceder de {size} px.',
            msgImageResizeError: 'No se pudieron obtener las dimensiones de la imagen para cambiar el tamaño.',
            msgImageResizeException: 'Error al cambiar el tamaño de la imagen.<pre>{errors}</pre>',
            msgAjaxError: 'Algo ha ido mal con la operación {operation}. Por favor, inténtelo de nuevo mas tarde.',
            msgAjaxProgressError: 'La operación {operation} ha fallado',
            msgDuplicateFile: 'File "{name}" of same size "{size} KB" has already been selected earlier. Skipping duplicate selection.',
            msgResumableUploadRetriesExceeded: 'Upload aborted beyond <b>{max}</b> retries for file <b>{file}</b>! Error Details: <pre>{error}</pre>',
            msgPendingTime: '{time} remaining',
            msgCalculatingTime: 'calculating time remaining',
            ajaxOperations: {
                deleteThumb: 'Archivo borrado',
                uploadThumb: 'Archivo subido',
                uploadBatch: 'Datos subidos en lote',
                uploadExtra: 'Datos del formulario subidos '
            },
            dropZoneTitle: 'Arrastre y suelte aquí los archivos &hellip;',
            dropZoneClickTitle: '<br>(o haga clic para seleccionar {files})',
            fileActionSettings: {
                removeTitle: 'Eliminar archivo',
                uploadTitle: 'Subir archivo',
                uploadRetryTitle: 'Reintentar subir',
                downloadTitle: 'Descargar archivo',
                zoomTitle: 'Ver detalles',
                dragTitle: 'Mover / Reordenar',
                indicatorNewTitle: 'No subido todavía',
                indicatorSuccessTitle: 'Subido',
                indicatorErrorTitle: 'Error al subir',
                indicatorPausedTitle: 'Upload Paused',
                indicatorLoadingTitle: 'Subiendo &hellip;'
            },
            previewZoomButtonTitles: {
                prev: 'Anterior',
                next: 'Siguiente',
                toggleheader: 'Mostrar encabezado',
                fullscreen: 'Pantalla completa',
                borderless: 'Modo sin bordes',
                close: 'Cerrar vista detallada'
            }
        });


        //
        // Custom layout
        //

        //$('.file-input-custom').fileinput({
        //    previewFileType: 'image',
        //    browseLabel: 'Select',
        //    browseClass: 'btn bg-slate-700',
        //    browseIcon: '<i class="icon-image2 mr-2"></i>',
        //    removeLabel: 'Remove',
        //    removeClass: 'btn btn-danger',
        //    removeIcon: '<i class="icon-cancel-square mr-2"></i>',
        //    uploadClass: 'btn bg-teal-400',
        //    uploadIcon: '<i class="icon-file-upload mr-2"></i>',
        //    layoutTemplates: {
        //        icon: '<i class="icon-file-check"></i>',
        //        modal: modalTemplate
        //    },
        //    initialCaption: "Please select image",
        //    mainClass: 'input-group',
        //    previewZoomButtonClasses: previewZoomButtonClasses,
        //    previewZoomButtonIcons: previewZoomButtonIcons,
        //    fileActionSettings: fileActionSettings
        //});


        //
        // Template modifications
        //

        //$('.file-input-advanced').fileinput({
        //    browseLabel: 'Browse',
        //    browseClass: 'btn btn-light',
        //    removeClass: 'btn btn-light',
        //    uploadClass: 'btn bg-success-400',
        //    browseIcon: '<i class="icon-file-plus mr-2"></i>',
        //    uploadIcon: '<i class="icon-file-upload2 mr-2"></i>',
        //    removeIcon: '<i class="icon-cross2 font-size-base mr-2"></i>',
        //    layoutTemplates: {
        //        icon: '<i class="icon-file-check"></i>',
        //        main1: "{preview}\n" +
        //            "<div class='input-group {class}'>\n" +
        //            "   <div class='input-group-prepend'>\n" +
        //            "       {browse}\n" +
        //            "   </div>\n" +
        //            "   {caption}\n" +
        //            "   <div class='input-group-append'>\n" +
        //            "       {upload}\n" +
        //            "       {remove}\n" +
        //            "   </div>\n" +
        //            "</div>",
        //        modal: modalTemplate
        //    },
        //    initialCaption: "No file selected",
        //    previewZoomButtonClasses: previewZoomButtonClasses,
        //    previewZoomButtonIcons: previewZoomButtonIcons,
        //    fileActionSettings: fileActionSettings
        //});


        //
        // Custom file extensions
        //

        //$('.file-input-extensions').fileinput({
        //    browseLabel: 'Browse',
        //    browseIcon: '<i class="icon-file-plus mr-2"></i>',
        //    uploadIcon: '<i class="icon-file-upload2 mr-2"></i>',
        //    removeIcon: '<i class="icon-cross2 font-size-base mr-2"></i>',
        //    layoutTemplates: {
        //        icon: '<i class="icon-file-check"></i>',
        //        modal: modalTemplate
        //    },
        //    maxFilesNum: 10,
        //    allowedFileExtensions: ["jpg", "gif", "png", "txt"],
        //    initialCaption: "No file selected",
        //    previewZoomButtonClasses: previewZoomButtonClasses,
        //    previewZoomButtonIcons: previewZoomButtonIcons,
        //    fileActionSettings: fileActionSettings
        //});


        //
        // Always display preview
        //

        //$('.file-input-preview').fileinput({
        //    browseLabel: 'Browse',
        //    browseIcon: '<i class="icon-file-plus mr-2"></i>',
        //    uploadIcon: '<i class="icon-file-upload2 mr-2"></i>',
        //    removeIcon: '<i class="icon-cross2 font-size-base mr-2"></i>',
        //    layoutTemplates: {
        //        icon: '<i class="icon-file-check"></i>',
        //        modal: modalTemplate
        //    },
        //    initialPreview: [
        //        '../../../../global_assets/images/placeholders/placeholder.jpg',
        //        '../../../../global_assets/images/placeholders/placeholder.jpg',
        //    ],
        //    initialPreviewConfig: [
        //        {caption: 'Jane.jpg', size: 930321, key: 1, url: '{$url}', showDrag: false},
        //        {caption: 'Anna.jpg', size: 1218822, key: 2, url: '{$url}', showDrag: false}
        //    ],
        //    initialPreviewAsData: true,
        //    overwriteInitial: false,
        //    maxFileSize: 100,
        //    previewZoomButtonClasses: previewZoomButtonClasses,
        //    previewZoomButtonIcons: previewZoomButtonIcons,
        //    fileActionSettings: fileActionSettings
        //});


        //
        // Display preview on load
        //

        //$('.file-input-overwrite').fileinput({
        //    browseLabel: 'Browse',
        //    browseIcon: '<i class="icon-file-plus mr-2"></i>',
        //    uploadIcon: '<i class="icon-file-upload2 mr-2"></i>',
        //    removeIcon: '<i class="icon-cross2 font-size-base mr-2"></i>',
        //    layoutTemplates: {
        //        icon: '<i class="icon-file-check"></i>',
        //        modal: modalTemplate
        //    },
        //    initialPreview: [
        //        '../../../../global_assets/images/placeholders/placeholder.jpg',
        //        '../../../../global_assets/images/placeholders/placeholder.jpg'
        //    ],
        //    initialPreviewConfig: [
        //        {caption: 'Jane.jpg', size: 930321, key: 1, url: '{$url}'},
        //        {caption: 'Anna.jpg', size: 1218822, key: 2, url: '{$url}'}
        //    ],
        //    initialPreviewAsData: true,
        //    overwriteInitial: true,
        //    previewZoomButtonClasses: previewZoomButtonClasses,
        //    previewZoomButtonIcons: previewZoomButtonIcons,
        //    fileActionSettings: fileActionSettings
        //});


        //
        // AJAX upload
        //

        //$('.file-input-ajax').fileinput({
        //    browseLabel: 'Browse',
        //    uploadUrl: "http://localhost", // server upload action
        //    uploadAsync: true,
        //    maxFileCount: 5,
        //    initialPreview: [],
        //    browseIcon: '<i class="icon-file-plus mr-2"></i>',
        //    uploadIcon: '<i class="icon-file-upload2 mr-2"></i>',
        //    removeIcon: '<i class="icon-cross2 font-size-base mr-2"></i>',
        //    fileActionSettings: {
        //        removeIcon: '<i class="icon-bin"></i>',
        //        removeClass: '',
        //        uploadIcon: '<i class="icon-upload"></i>',
        //        uploadClass: '',
        //        zoomIcon: '<i class="icon-zoomin3"></i>',
        //        zoomClass: '',
        //        indicatorNew: '<i class="icon-file-plus text-success"></i>',
        //        indicatorSuccess: '<i class="icon-checkmark3 file-icon-large text-success"></i>',
        //        indicatorError: '<i class="icon-cross2 text-danger"></i>',
        //        indicatorLoading: '<i class="icon-spinner2 spinner text-muted"></i>',
        //    },
        //    layoutTemplates: {
        //        icon: '<i class="icon-file-check"></i>',
        //        modal: modalTemplate
        //    },
        //    initialCaption: 'No file selected',
        //    previewZoomButtonClasses: previewZoomButtonClasses,
        //    previewZoomButtonIcons: previewZoomButtonIcons
        //});


        //
        // Misc
        //

        // Disable/enable button
        //$('#btn-modify').on('click', function() {
        //    $btn = $(this);
        //    if ($btn.text() == 'Disable file input') {
        //        $('#file-input-methods').fileinput('disable');
        //        $btn.html('Enable file input');
        //        alert('Hurray! I have disabled the input and hidden the upload button.');
        //    }
        //    else {
        //        $('#file-input-methods').fileinput('enable');
        //        $btn.html('Disable file input');
        //        alert('Hurray! I have reverted back the input to enabled with the upload button.');
        //    }
        //});
    };


    //
    // Return objects assigned to module
    //

    return {
        init: function() {
            _componentFileUpload();
        }
    }
}();


// Initialize module
// ------------------------------

document.addEventListener('DOMContentLoaded', function() {
    FileUpload.init();
});
