module.exports = function (context, myBlob) {
    context.log("Node.js blob trigger function processed blob \n Name:", context.bindingData.name, "\n Blob Size:", myBlob.length, "Bytes");
    context.log(JSON.stringify(context));
    context.log(JSON.stringify(context));
    var blobSvc = azure.createBlobService();
    blobSvc.deleteBlob(containerName, 'myblob', function(error, response){
        if(!error){
            // Blob has been deleted
            context.log('deleted')
        }
    });
    context.done();
};