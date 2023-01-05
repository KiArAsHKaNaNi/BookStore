var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        ajax: {
            url: "/User/GetAll"
        },
        columns: [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "company.name", "width": "15%" },
            { "data": "role", "width": "15%" },
            {
                "data": {
                    id: "id",
                    lockoutEnd: "lockoutEnd"
                },
                "render": function (data) {
                    let today = new Date().getTime();
                    let lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        //user is currently locked
                        return `
                             <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-danger" style = "width:100px;">
                                    Unlock 
                                </a>
                            </div>

                        `;
                    } else {
                        return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-success" style = "width:100px;">
                                    Lock 
                                </a>
                            </div>

                        `;
                    }
                },

                "width": "25%"
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

function LockUnlock(id) {
    $.ajax({
        type: "Post",
        url: "/User/LockUnlock",
        data: JSON.stringify(id),
        contentType: "application/json",
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