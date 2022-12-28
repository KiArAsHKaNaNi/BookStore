var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Category/GetAll"
        },
        "columns": [
            { "data": "name", "width": "60%" },
            {
                "data": "categoryid",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Category/Upsert/${data}" class="btn btn-success">
                                    Edit
                                </a>
                                <a onclick=Delete("/Category/Delete/${data}") class="btn btn-danger">
                                    Delete
                                </a>
                            </div>

                        `;
                },
                "width": "40%"
            }
        ]

    });
}

//function Delete(url) {
//    swal({
//        title: "Are you sure?",
//        text: "Once deleted, you will not be able to restore the data!",
//        icon: "warning",
//        buttons: true,
//        dangerMode: true,
//    }).then(willDelete => {
//        if (willDelete) {
//            $.ajax({
//                type: "DELETE",
//                url: url,
//                success: function (data) {
//                    if (data.success) {
//                        toastr.success(data.message);
//                        dataTable.ajax.reload();
//                    } else {
//                        toastr.error(data.message);
//                    }
//                }

//            })
//        }

//    })


//}