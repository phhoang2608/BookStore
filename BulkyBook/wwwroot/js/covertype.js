var dataTable;

$(document).ready(function () {
    loadDataTable(); //basically will load the function when the document is ready.
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({ //make sure everything is in exact case because Js is case sensitive
        "ajax": { //loading the url using ajax call
            "url": "/Admin/CoverType/GetAll" //the url to get all coverType
        },
        "columns": [
            { "data": "name", "width": "60%" }, //lower case cause of camel casing
            {
                "data": "id",
                "render": function (data) //render 2 buttons or 2 links, parameters of data which is the id in this case
                {
                    //return the div that shows up the two buttons delete and upsert
                    return `
                    <div class="text-center">
                        <a href="/Admin/CoverType/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
                            <i class="fas fa-edit"></i>
                        </a>
                        <a onclick=Delete("/Admin/CoverType/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
                            <i class="fas fa-trash-alt"></i>
                        </a>
                    </div>
                    `
                }, "width": "40%"
            }
        ]
    });
}

function Delete(url) {
    swal({
        title: "Are you sure you want to delete?",
        text: "You will not be able to restore the data!",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });

}