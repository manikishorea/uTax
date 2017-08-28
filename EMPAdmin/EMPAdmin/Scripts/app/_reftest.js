$(document).ready(function () {
    getAllBooks();
});


function ajaxHelper(uri, method, data) {
    $('#error').hide();
    $('#error p').html('');
    return $.ajax({
        type: method,
        url: uri,
        dataType: 'json',
        contentType: 'application/json',
        data: data ? JSON.stringify(data) : null
    }).fail(function (jqXHR, textStatus, errorThrown) {
        //  self.error(errorThrown);
        $('#error').show();
        $('#error p').html(errorThrown);
    });
}



function getAllBooks() {
    var booksUri = '/api/Values/';
    var table = $('#table_book'); //$(document.body)
    ajaxHelper(booksUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var row = $("<tr/>").appendTo(table);;
            $.each(r, function (colIndex, c) {
                row.append($("<td/>").text(c));
            });
            row.append($("<td/>").html("<a href='#' onclick='getBookDetail(" + r['Id'] + ")'> Detail </a>"));
        });
    });
}



function getBookDetail(Id) {
    var booksUri = '/api/Values/';
    var divDetail = $('#div_bookdetail'); //$(document.body)

    var table = $('<table/>').addClass('table').attr('id','table_bookdetail'); //$(document.body)
    ajaxHelper(booksUri + Id, 'GET').done(function (data) {
        var row = $("<tr/>").appendTo(table);
        row.append($("<td/>").text('Author'));
        row.append($("<td/>").text(data["AuthorName"]));
        row = $("<tr/>").appendTo(table);
        row.append($("<td/>").text('Title'));
        row.append($("<td/>").text(data["Title"]));
        row = $("<tr/>").appendTo(table);
        row.append($("<td/>").text('Year'));
        row.append($("<td/>").text(data["Year"]));
        row = $("<tr/>").appendTo(table);
        row.append($("<td/>").text('Genre'));
        row.append($("<td/>").text(data["Genre"]));
        row = $("<tr/>").appendTo(table);
        row.append($("<td/>").text('Price'));
        row.append($("<td/>").text(data["Price"]));
    });
}


function getAuthors() {
    var authorsUri = '/api/authors/';
    ajaxHelper(authorsUri, 'GET').done(function (data) {
        // self.authors(data);
    });
}


function AddItem() {
    var booksUri = '/api/Values/';
    //Add New Item
    var book = {
        AuthorId: self.newBook.Author().Id,
        Genre: self.newBook.Genre(),
        Price: self.newBook.Price(),
        Title: self.newBook.Title(),
        Year: self.newBook.Year()
    };

    ajaxHelper(booksUri, 'POST', book).done(function (item) {
        //self.books.push(item);
    });
}




