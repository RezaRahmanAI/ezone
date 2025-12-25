$(document).ready(function () {
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    });

    $('a').on('click', function () {
        $(this).find('i[class*="fa fa-angle-right"]').toggleClass('fa-rotate-90');
        //alert("clicked");
    });
});

// Start
// For expanding and coloring the  visited link
$('a').on('click', function () {
    var classname = $(this).attr('class');
    var id = $(this).attr('id');

    if (classname == 'dropdown-toggle') {
        openNav(id);
    }
    else {
        ColorNav(id);
    }
    //alert("clicked");
});

function ColorNav(id) {
    if (typeof (Storage) !== "undefined") {
        // Save the state of the sidebar as "open"
        localStorage.setItem("colorsidebar", id);
    }
}

function openNav(id) {
    // If localStorage is supported by the browser
    if (typeof (Storage) !== "undefined") {
        // Save the state of the sidebar as "open"
        localStorage.setItem("sidebar", id);
    }
}


if (typeof (Storage) !== "undefined") {
  
    // If we need to open the bar
    var ID = localStorage.getItem("sidebar");
    if (ID != undefined) {
        // Open the bar
        var id = '#' + ID;
        var parentid = $(id).closest('ul').attr('id');
        if (parentid != undefined) {
            var imid = '#id' + parentid;
            $(imid + '~ ul').attr('id');
            $(imid + '~ ul').attr('class', 'list-unstyled in');
            $(imid + '~ ul').attr('height', '0px');
        }
        else {
            var imid = '#' + $(id + '~ ul').attr('id');
            $(imid).attr('class', 'list-unstyled in');
            $(imid).attr('height', '0px');
        }

        var childId = $(id + '~ ul').attr('id');
        if (childId != undefined) {
            var imid = '#id' + childId;
            $(imid + '~ ul').attr('id');
            $(imid + '~ ul').attr('class', 'list-unstyled in');
            $(imid + '~ ul').attr('height', '0px');
            $(id).find('i[class*="fa fa-angle-right"]').toggleClass('fa-rotate-90');
        }
    }
    else
        $('#sidebar').addClass('active');

    var colorMenueId = localStorage.getItem("colorsidebar");
    if (colorMenueId != undefined) {
        $('#' + colorMenueId).css('color', '#cddc39');
    }
}

// For expanding and coloring the  visited link
// End