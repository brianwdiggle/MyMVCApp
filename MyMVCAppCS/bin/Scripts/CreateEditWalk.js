/*-------------------------------------------------------------------------------------------------------------------------------
/
/ Script: CreateEditWalk.js
/
/ Description: Script which implements client side logic to support creation and editing of walks
/
/ Dependencies:
/    1. jQuery 2.1.1
/    2. jQuery UI 1.11.1
/    3. jQuery Validation Plug-in v1.12.0
/
/-----------------------------------------------------------------------------------------------------------------------------*/
function getHome() {
    return document.getElementById("ApplicationRoot").getAttribute("href");
}

$(function () {

 
     
    /*----Associate an autocomplete with the marker left on hill box which will make an AJAX call-----*/
    $("#MarkerLeftOnHill").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: getHome() +  "Walks/HillSuggestions",
                dataType: "json",
                data: {
                    term: request.term,
                    areaid: $("#WalkAreaID").val()
                },
                success: function (data) {
                    response(data);
                }
            });
        },
        minLength: 2,
        appendTo: "#MarkerModalDialogForm",
        select: function (event, ui) {
            var hillid = extractid(ui.item.value);
            $("#MarkerLeftOnHillId").val(hillid);
        }
    });

    //---This function is used to populate the hidden html field WalkAreaID which is used by the summit autocomplete to narrow suggestions to the right area
    function extractareaid(walkareaname) {

        if (walkareaname.indexOf("|") > 0) {
            var myloc = walkareaname.indexOf("|");
            $("#WalkAreaID").val(walkareaname.substring(myloc + 1).trim());
        }
    }

    $("#WalkAreaName").autocomplete({
        source: getHome() + "Walks/WalkAreaSuggestions",
        minLength: 2,
        select: function (event, ui) {
            extractareaid(ui.item.value);
        }
    });

    /*----Associate an autocomplete widget with each of the hill visited text boxes. Each hill visited text box has an----*/
    /*----matching hidden field which will hold a selected hill id-------*/

    var numberofascents = 0;    // Used by Edit walk to make sure existing ascents are not hidden

    for (var iSummitVisitCount = 1; iSummitVisitCount <= 15; iSummitVisitCount = iSummitVisitCount + 1) {

        $("#VisitedSummit" + iSummitVisitCount).autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: getHome() + "Walks/HillSuggestions",
                    dataType: "json",
                    data: {
                        term: request.term,
                        areaid: $("#WalkAreaID").val()
                    },
                    success: function (data) {
                        response(data);
                    }
                });
            },
            minLength: 2,
            select: function (event, ui) {
                var summitnumber = extractnumber("VisitedSummit", event.target.id);
                var hillid = extractid(ui.item.value);
                $("#DivVisitedSummit" + (parseInt(summitnumber) + 1)).show();
                $("#VisitedSummit" + (parseInt(summitnumber) + 1)).focus();
                $("#VisitedSummit" + summitnumber + "HillID").val(hillid);
            }
        });

        // Keep a note of the number of ascents
        if ($("#VisitedSummit" + iSummitVisitCount + "HillID").val().length > 0) {
            numberofascents++;
        }
    }

    // Leave the next unused ascent textbox visible for editing, hide all the rest
    for (var ascentstohide = numberofascents + 2; ascentstohide <= 15; ascentstohide++) {
        $("#DivVisitedSummit" + ascentstohide).hide();
    }

    //*------Implementation of association of marker with image---------
    //*------Associate an autocomplete jQuery UI widget with all DOM elements with class "markersuggestions"---------
    $(document).on("keydown.autocomplete", ".markersuggestions", function () {

        $(this).autocomplete({
            source: getHome()+ "Walks/MarkerSuggestions",
            minLength: 2,
            select: function (event, ui) {
                var elementroot = event.target.id.replace(/[0-9]/g, '');
                var imagenumber = extractnumber(elementroot, event.target.id);
                var markerid = extractid(ui.item.value);
                if (elementroot.indexOf("existing") >=0) {
                    $("#existingimagemarkerid" + imagenumber).val(markerid);
                } else {
                    $("#imagemarkerid" + imagenumber).val(markerid);
                }
            }
        });

    });

    //*---When the "image is marker" checkbox is clicked, reveal the marker details span------
    $(document).on("click", ".imageismarker:checked", function (e) {
        var elementroot = e.target.id.replace(/[0-9]/g, '');
        var markerimage = extractnumber(elementroot, e.target.id);
        if (elementroot.indexOf("existing") >= 0) {
            $("#existingimagemarkerdetails" + markerimage).show();
        } else {
            $("#imagemarkerdetails" + markerimage).show();
        }
       
    });

    //*----Hide marker details span for existing images when "Marker?" checkbox is not checked-----
    $(".imageismarker:not(:checked)").each(function(index) {
        var existingimagenumber = extractnumber("existingimageismarker", this.id);
        $("#existingimagemarkerdetails" + existingimagenumber).hide();
    });

    //*-----The Json results from the autocomplete remote source contain also the hill or marker ID. 
    //*-----This function extracts the ID from this and returns it
    function extractnumber(removestr, sourcestring) {
        return sourcestring.replace(removestr, "");
    }


    function extractid(elementname) {
        if (elementname.indexOf("|") > 0) {
            var myloc = elementname.indexOf("|");
            return elementname.substring(myloc + 1).trim();
        }
        return "";
    }

    //---Function used in CreateAuto to check that files exist in the specified directory
    $('#walkfiles_reldir').change(function (e) {

        $.getJSON(getHome() + 'Walks/CreateAutoCheckFiles', { reldir: $('#walkfiles_reldir').val() }, function (oResults) {
            if (oResults.error.length > 0) {
                alert(oResults.error);
                $('#walkfiles_reldir').focus();
            }
        });

    });

    /*----Associate a AJAX call (AJAJ in fact as it returns JSON) with the blur event of the auxilliary_file1 */
    /*----How to write a generic handler?------------------------------*/

    $('.auxilliaryfileclass').change(function (e) {

        var auxilliaryfilenumber = extractnumber("auxilliary_file", e.target.id);

        $.getJSON(getHome() + 'Walks/CheckFileInWebrootJSON', { imagepath: $('#auxilliary_file' + auxilliaryfilenumber).val() }, function (oResults) {
            if (oResults.isinpath == "False") {
                alert('The file you specified is either not in the web site root or does not exist');
                $('#auxilliary_file' + auxilliaryfilenumber).val("");
            } else {
                $('#auxilliary_filesdiv' + (parseInt(auxilliaryfilenumber) + 1)).show();
            }
        });

    });
});

$(document).ready(function () {

    /*---Calculate the overall speed where possible-------*/
    $("#CartographicLength").focusout(function () {
        calculateOverallSpeed();
    });
    $("#total_time_hours").focusout(function () {
        calculateOverallSpeed();
    });
    $("#total_time_mins").focusout(function () {
        calculateOverallSpeed();
    });

    /*---Empty the autocomplete text field on click----*/
    $('#WalkAreaName').click(function () {
        $('#WalkAreaName').val("");
    });

    /*----Hide the currently unused auxilliary file sections------*/
    for (var iAuxFileCount = 2; iAuxFileCount <= 6; iAuxFileCount = iAuxFileCount + 1) {
        $('#auxilliary_filesdiv' + iAuxFileCount).hide();
    }

    /*----Associate a datepicker widget ( from jQuery/UI/DatePicker ) with the WalkDate text box----*/
    $(function () {
        $("#WalkDate").datepicker({ dateFormat: 'dd MM yy' });
    });

    /*----When "check images" is clicked, make AJAX call to see how many matching images are found.----*/
    /*----Then insert the images together with ImageCaption text boxes into the DOM------*/
    $('#getimages').click(function () {

        $.get(getHome() + 'Walks/CheckImages', { imagepath: $("#images_path").val() }, function (oResults) {

            for (var iImageCount = 1; iImageCount <= oResults.imagesfound; iImageCount = iImageCount + 1) {

                $("#walkimages").append('<br/><b>Image ' + iImageCount + '</b><br/><input type="text" id="imagecaption' + iImageCount + '" name="imagecaption' + iImageCount + '" size="100" />&nbsp;Marker? <input type="checkbox" class="imageismarker" id="imageismarker' + iImageCount + '" name="imageismarker' + iImageCount + '"/>');

                if (oResults.atwork == "True") {
                    $("#walkimages").append("&nbsp;" + oResults.filenameprefix + iImageCount + '.jpg</br>');
                } else {
                    $("#walkimages").append('<br/><img src="' + oResults.path + iImageCount + '.jpg" border="1" />');
                }
                $("#walkimages").append('<input type="hidden" id="imagerelpath' + iImageCount + '" name="imagerelpath' + iImageCount + '" value="' + oResults.path + iImageCount + '.jpg"/><br/>');

                //----Add a hidden marker section, revealed when clicking the "imageismarker" checkbox-----------
                var markermarkup = '<span id="imagemarkerdetails' + iImageCount + '">' +
                    '<br/>Marker name: ' + '<input type="text" size="50" name="imagemarkername' + iImageCount + '" id="imagemarkername' + iImageCount + '" class="markersuggestions" />' +
                    '<input type="hidden" id="imagemarkerid' + iImageCount + '" name="imagemarkerid' + iImageCount + '" />' +
                    'Not Found? <input type="checkbox" id="imagemarkernotfound' + iImageCount + '" name="imagemarkernotfound' + iImageCount + '" /></span>';

                $("#walkimages").append(markermarkup);
                $("#imagemarkerdetails" + iImageCount).hide();
            }
            $("#images_path").focus();
        });

    });

    /*----If the walk is circular, put the start location in the end location field */
    $('#WalkStartPoint').keyup(function () {
        if ($('#WalkTypes').val() == 'Mountain  - Circular' || $('#WalkTypes').val() == 'Valley - Circular') {
            $('#WalkEndPoint').val($('#WalkStartPoint').val());
        }

    });

    /*----Now follows...the modal form for marker creation------------------------*/
    var markertitle = $("#MarkerTitle"),
        markerdateleft = $("#MarkerDateLeft"),
        markerdescription = $("#MarkerDescription"),
        markerhill = $("#MarkerLeftOnHill"),
        markerhillid = $("#MarkerLeftOnHillId"),
        markergps = $("#MarkerGPSReference"),
        allFields = $([]).add(markertitle).add(markerdateleft).add(markerdescription).add(markerhillid).add(markergps).add(markerhill),
        tips = $(".validateMarkerFormTips");

    function updateTips(t) {
        tips
            .text(t)
            .addClass('ui-state-highlight');
        setTimeout(function () {
            tips.removeClass('ui-state-highlight', 1500);
        }, 500);
    }

    function checkLength(o, n, min, max) {

        if (o.val().length > max || o.val().length < min) {
            o.addClass('ui-state-error');
            updateTips("Length of " + n + " must be between " + min + " and " + max + ".");
            return false;
        } else {
            return true;
        }

    }

    function calculateOverallSpeed() {
        if ($("#CartographicLength").val() != "" && ($("#total_time_hours").val() != "" || $("#total_time_mins").val() != "")) {

            var numhours = 0;
            if ($("#total_time_hours").val() != "") {
                numhours += parseFloat($("#total_time_hours").val());
            }
            if ($("#total_time_mins").val() != "") {
                numhours += parseFloat($("#total_time_mins").val()) / 60;
            }

            var overallspeed = parseFloat($("#CartographicLength").val()) / numhours;
            $("#WalkAverageSpeedKmh").val(overallspeed.toPrecision(2));
        }
    }

    /*----Add a modal form which is used to capture the data for a new marker. Using jQuery/UI/Dialog---------*/
    $("#MarkerModalDialogForm").dialog({
        autoOpen: false,
        height: 630,
        width: 700,
        zIndex: 10000000,  //To ensure that the drop-down of suggestions appears in front of everything else
        modal: false,
        buttons: {
            'Create Marker': function () {
                allFields.removeClass('ui-state-error');
                var bValid;
                bValid = checkLength(markertitle, "marker title", 5, 80);
                bValid = bValid && checkLength(markerdescription, "description", 6, 1000);
                bValid = bValid && checkLength(markerdateleft, "date left", 5, 18);

                if (bValid) {

                    /*-----Do AJAX call to add insert the new marker so that its available immediately for selection----*/
                    $.getJSON(getHome() + 'Walks/CreateMarker', { mtitle: markertitle.val(), mdesc: markerdescription.val(), mdate: markerdateleft.val(), mhillid: markerhillid.val(), mgps: markergps.val() },
                        function (oResults, status) {
                            if (status == "success") {
                                $('#WalkMarkers').append('<br/>&nbsp;<br/><table class="markercreated"><tr><td colspan="2"><strong>Marker Created</strong></td></tr>' +
                                '<tr><td><em>Title:</em></td><td>' + markertitle.val() + '</td></tr><tr><td><em>Date Left:</em></td><td>' +
                                markerdateleft.val() + '</td></tr><tr><td>Description:</td><td>' + markerdescription.val().replace(/(?:\r\n|\r|\n)/g, '<br />') + '</td></tr></table>');
                                var mi = $("#markers_added").val();
                                $("#markers_added").val(mi + ":" + oResults.markerid);
                                $("#MarkerModalDialogForm").dialog('close');
                            } else {
                                $(".validateMarkerFormTips").val = "An error occurred when inserting the new marker: " + status;
                            }
                            allFields.val('').removeClass('ui-state-error');
                        });
                }
            },
            Cancel: function () {
                $(this).dialog('close');
            }
        },
        close: function () {
            allFields.val('').removeClass('ui-state-error');
        }
    });

    /*----Add a Create Marker button to the form. Using jQuery/UI/Button -------*/
    $('#CreateMarkerButton')
        .button()
        .click(function () {
            $('#MarkerDateLeft').val($('#WalkDate').val());
            $('#MarkerModalDialogForm').dialog('open');
        });

    /*----Create event for main button click----*/
    $('#submitwalkbutton').button().click(function () {
        $('#walkform').submit();
    });

    /*----Associate a date picker with the marker left date----*/
    $("#MarkerDateLeft").datepicker({ dateFormat: 'dd MM yy' });

    /*----Set up form validation with the jQuery Validation plugin-------*/
    $("#walkform").validate({
        rules: {
            WalkDescription: {
                minlength: 10,
                required: true
            },
            WalkAreaName: {
                required: true
            },
            WalkTitle: {
                required: true,
                minlength: 5
            },
            WalkDate: {
                required: true
            },
            WalkStartPoint: {
                required: true,
                minlength: 3
            },
            WalkEndPoint: {
                required: true,
                minlength: 3
            },
            CartographicLength: {
                number: true,
                range: [0.1, 60]
            },
            MetresOfAscent: {
                number: true,
                range: [0, 8848]
            },
            total_time_hours: {
                number: true,
                range: [0, 18]
            },
            total_time_mins: {
                number: true,
                range: [0, 59]
            },
            WalkAverageSpeedKmh: {
                number: true,
                range: [0, 8]
            },
            MovingAverageKmh: {
                number: true,
                range: [0, 8]
            }
        }
    });
});