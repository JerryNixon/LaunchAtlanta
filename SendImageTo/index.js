var azure = require('azure-storage');

module.exports = function (context, myBlob) {
    var container = "workitems";
    context.log("Node.js blob trigger function processed blob \n Name:", context.bindingData.name, "\n Blob Size:", myBlob.length, "Bytes");
    var blobName = context.bindingData.name;
    context.log(JSON.stringify(context));
    context.log(JSON.stringify(context));
    var blobSvc = azure.createBlobService();
    blobSvc.deleteBlob(container, blobName, function(error, response){
        if(!error){
            // Blob has been deleted
            context.log('deleted')
        }
    });
    context.done();
};