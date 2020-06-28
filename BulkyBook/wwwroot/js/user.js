var dataTable;

$(document).ready(function () {
    loadDataTable(); //basically will load the function when the document is ready.
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({ //make sure everything is in exact case because Js is case sensitive
        "ajax": { //loading the url using ajax call
            "url": "/Admin/User/GetAll" //the url to get all category
        },
        "columns": [
            { "data": "name", "width": "15%" }, //lower case cause of camel casing
            { "data": "email", "width": "15%" }, //lower case cause of camel casing
            { "data": "phoneNumber", "width": "15%" }, //lower case cause of camel casing
            { "data": "company.name", "width": "15%" }, //lower case cause of camel casing
            { "data": "role", "width": "15%" }, //lower case cause of camel casing
            {
                "data": {
                    id: "id", lockOutEnd: "lockoutEnd"
                },
                "render": function (data) //render 2 buttons or 2 links, parameters of data which is the id in this case
                {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        //this means the user is currently being locked
                        return `
                        <div class="text-center">
                            <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:100px;">
                                <i class="fas fa-lock-open"></i> 
                                Unlock
                            </a>
                        </div>
                    `;
                    }
                    else {
                        return `
                        <div class="text-center">
                            <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width:100px;">
                                <i class="fas fa-lock"></i>
                                Lock
                            </a>
                        </div>
                    `;
                    }
                  
                }, "width": "25%"
            }
        ]
    });
}

function LockUnlock(id) {
                $.ajax({
                    type: "POST",
                    url: "/Admin/User/LockUnlock",
                    data: JSON.stringify(id),
                    contentType: "application/json",
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