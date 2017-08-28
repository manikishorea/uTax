function makeTable(container, data) {
    var table = $("<table/>").addClass('table');
    $.each(data, function (rowIndex, r) {
        var row = $("<tr/>");
        $.each(r, function (colIndex, c) {
            row.append($("<t" + (rowIndex == 0 ? "h" : "d") + "/>").text(c));
        });
        table.append(row);
    });
    return container.append(table);
}

function appendTableColumn(table, rowData) {
    var lastRow = $('<tr/>').appendTo(table.find('tbody:last'));
    $.each(rowData, function (colIndex, c) {
        lastRow.append($('<td/>').text(c));
    });

    return lastRow;
}

function appendTableRows(table, rowData) {
    $.each(rowData, function (rowIndex, r) {
        var row = $("<tr/>").appendTo(table);;
        $.each(r, function (colIndex, c) {
            row.append($("<td/>").text(c));
        });
       // table.append(row);
    });
    return "";
}

function appendTableHeader(table, rowData) {
    var lastRow = $('<thead><th/>').append(table);
    $.each(rowData, function (colIndex, c) {
        lastRow.append($('<th/><thead>').text(c));
    });

    return lastRow;
}

$(document).ready(function () {
    //  var data = [["City 1", "City 2", "City 3"], //headers
    //              ["New York", "LA", "Seattle"],
    //              ["Paris", "Milan", "Rome"],
    //               ["Pittsburg", "Wichita", "Boise"]]
    //  var table = $('#table_book'); //$(document.body)
    //  var cityTable = makeTable(table, data);

    //var cityTable = makeTable(table, msg);
    // var RowTable = appendTableRows(table, data);

    var select = $('#select_book');
    var data = {
        'foo': 'bar',
        'foo2': 'baz'
    }

    makeDropDown(select, data);
});



function makeDropDown(container, data) {

    var select = $('<select />');

    for (var val in data) {
        $('<option />', { value: val, text: data[val] }).appendTo(s);
    }

    container.appendTo(select);
}