$(document).ready(function () {
    var dataSend;
    var ID;
    var fname;
    var lname;
    var age;
    //clears text boxes and selects add mode for browsers that persist the information
    $("#4").val("");
    $("#3").val("");
    $("#2").val("");
    $("#1").val("");
    $("#add").prop("checked", true);
    //enables or disables the information entry text boxes to prevent user confusion when deleting data
    function inputEnable(enable) {
        $("#3").prop("disabled", enable)
        $("#2").prop("disabled", enable);
        $("#1").prop("disabled", enable);
    }

    //populates the info entry text boxes with the selected row and ensures modify is the mode selected
    $("tr").click(function () {
        inputEnable(false);
        var text = $(this).text();
        var trimmed = text.trim();
        ID = trimmed.slice(0, trimmed.indexOf(" "));
        $("#4").val(ID);
        var trimTwo = trimmed.slice(trimmed.indexOf(" "), trimmed.length).trim()
        fname = trimTwo.slice(0, trimTwo.indexOf(" "));
        $("#1").val(fname);
        lname = trimTwo.slice(trimTwo.indexOf(" "), trimTwo.lastIndexOf(" ")).trim();
        $("#2").val(lname);
        var trimThree = trimTwo.slice(trimTwo.indexOf(" "), trimTwo.length).trim();
        age = trimThree.slice(trimThree.indexOf(" "), trimThree.length).trim();
        $("#3").val(age);
        $("#modify").prop("checked", true);
        $("#modify").prop("disabled", false);
        $("#delete").prop("disabled", false);
        dataSend = "%" + ID + "," + fname + "," + lname + "," + age + ",";
    });

    $(document).ajaxComplete(function () { location.reload(); });
    //clears out any existing info in the info text boxes when the user wishes to add an entry
    $("#add").click(function () {
        inputEnable(false);
        $("#4").val("");
        $("#3").val("");
        $("#2").val("");
        $("#1").val("");
        $("#modify").prop("disabled", true);
        $("#delete").prop("disabled", true);
    })

    //if the user has already modified the selected information in the textboxes, this resets it to the actual information in the table and disables editing of those fields
    //this is just for confusion prevention, as the correct row would be deleted anyways
    $("#delete").click(function () {
        $("#3").val(age);
        $("#2").val(lname);
        $("#1").val(fname);
        inputEnable(true);
    })
    //allows the user to edit the information in the text boxes
    $("#modify").click(function () {
        inputEnable(false);
    });

    //sends an ajax post request to the server based on the mode selected and info provided, throws an error if any of the fields are blank
    $("#submit").click(function () {
        inputEnable(false);
        fname = $("#1").val().trim();
        lname = $("#2").val().trim();
        age = $("#3").val().trim();
        if ($("input:checked").val() == "add") {
            dataSend = fname + "," + lname + "," + age + ",";
        }
        if ($("input:checked").val() == "modify") {
            dataSend = "%" + ID + "," + fname + "," + lname + "," + age + ",";
        }
        if (fname != "" && lname != "" && age != "") {
            $.ajax({
                url: "http://localhost:8000/",
                method: "POST",
                data: dataSend + $("input:checked").val()
            })

        }
        else {
            alert("There must be a value for first name, last name, and age");
        }
    })
})



