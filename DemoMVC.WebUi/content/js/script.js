$(document).ready(function(){
    
    $('select').wrap('<div class="select_dropdown"></div>');
    $('.datepicker').datepicker({ autoclose: true, todayHighlight: true });

    //File Upload
    $('input[type="file"]').on('change', function() {
        var input = $(this);
        if (input.val()) {
            var label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
            $(this).parent().parent().find('.selected_filename').text(label).addClass('active');
        }
        else {
            $(this).parent().parent().find('.selected_filename').text('Please select file');
        }
    });
  
})
