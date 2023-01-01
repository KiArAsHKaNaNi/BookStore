var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        ajax: {
            url: "/Product/GetAll"
        },
        columns: [
            { "data": "title", "width": "15%" },
            { "data": "isbn", "width": "15%" },
            { "data": "price", "width": "15%" },
            { "data": "author", "width": "15%" },
            { "data": 'category.name', "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Product/Upsert/${data}" class="btn btn-success">
                                    Edit
                                </a>
                                <a onclick=Delete("/Product/Delete/${data}") class="btn btn-danger">
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
//    Swal.fire({
//        title: 'Are you sure?',
//        text: "You won't be able to revert this!",
//        icon: 'warning',
//        showCancelButton: true,
//        confirmButtonColor: '#3085d6',
//        cancelButtonColor: '#d33',
//        confirmButtonText: 'Yes, delete it!'
//    }).then(willDelete => {
//        if (willDelete.isConfirmed) {
//            $.ajax({
//                type: "DELETE",
//                url: url,
//                success: function (data) {
//                    if (data.success) {
//                        Swal.fire({
//                            position: 'top-end',
//                            icon: 'success',
//                            title: data.message,
//                            showConfirmButton: false,
//                            timer: 1500
//                        })
//                        dataTable.ajax.reload();
//                    } else {
//                        Swal.fire({
//                            position: 'top-end',
//                            icon: 'error',
//                            title: data.message,
//                            showConfirmButton: false,
//                            timer: 1500
//                        })
//                    }
//                }

//            })
//        }
//    }
//}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then(willDelete => {
        if (willDelete.isConfirmed) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        Swal.fire({
                            position: 'top-end',
                            icon: 'success',
                            title: data.message,
                            showConfirmButton: false,
                            timer: 1500
                        })
                        dataTable.ajax.reload();
                    } else {
                        Swal.fire({
                            position: 'top-end',
                            icon: 'error',
                            title: data.message,
                            showConfirmButton: false,
                            timer: 1500
                        })
                    }
                }

            })
        }

    })


}