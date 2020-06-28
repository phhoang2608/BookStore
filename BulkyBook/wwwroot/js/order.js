var dataTable;

$(document).ready(function () {
    //since we load based on url differences
    var url = window.location.search;
    //we load based on keywords inside the string 
    //basically will load the function when the document is ready.
    if (url.includes("inprocess")) {
        loadDataTable("GetOrderList?status=inprocess");
    }
    else if (url.includes("pending")) {
        loadDataTable("GetOrderList?status=pending");
    }
    else if (url.includes("completed")) {
        loadDataTable("GetOrderList?status=completed");
    }
    else if (url.includes("rejected")) {
        loadDataTable("GetOrderList?status=rejected");
    }
    else
    {
        loadDataTable("GetOrderList?status=all")
    }
});

function loadDataTable(url) {
    dataTable = $('#tblData').DataTable({ //make sure everything is in exact case because Js is case sensitive
        "ajax": { //loading the url using ajax call
            "url": "/Admin/Order/" + url //the url to get all category
        },
        "columns": [
            { "data": "id", "width": "15%" }, //lower case cause of camel casing
            { "data": "name", "width": "15%" }, //lower case cause of camel casing
            { "data": "phoneNumber", "width": "15%" }, //lower case cause of camel casing
            { "data": "applicationUser.email", "width": "15%" }, //lower case cause of camel casing        
            { "data": "orderStatus", "width": "15%" }, //lower case cause of camel casing
            { "data": "orderTotal", "width": "15%" }, //lower case cause of camel casing
            {
                "data": "id",
                "render": function (data) //render 2 buttons or 2 links, parameters of data which is the id in this case
                {
                    //return the div that shows up the two buttons delete and upsert
                    return `
                    <div class="text-center">
                        <a href="/Admin/Order/Details/${data}" class="btn btn-success text-white" style="cursor:pointer">
                            <i class="fas fa-edit"></i>
                        </a>
                    </div>
                    `
                }, "width": "5%"
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