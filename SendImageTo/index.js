var azure = require('azure-storage');
var fs = require("fs");
var Twitter = require('twitter');
var path = require('path');

module.exports = function (context, myBlob) {
    var containerName = "workitems";
    var destinationFileNameTarget = path.join(os.tmpdir(),"file.jpg");

    var client = new Twitter({
        consumer_key: process.env.TWITTER_CONSUMER_KEY,
        consumer_secret: process.env.TWITTER_CONSUMER_SECRET,
        access_token_key: process.env.TWITTER_ACCESS_TOKEN_KEY,
        access_token_secret: process.env.TWITTER_ACCESS_TOKEN_SECRET
    });

    context.log("Node.js blob trigger function processed blob \n Name:", context.bindingData.name, "\n Blob Size:", myBlob.length, "Bytes");
    context.log('Node.js blob trigger function processed blob', myBlob);
    //context.log('Node.js blob type of',  typeOf(myBlob));

    var blobName = context.bindingData.name;
    var blobSvc = azure.createBlobService();

    // Load your image
    var stream = fs.createReadStream(fileNameTarget).pipe(blobService.createWriteStreamToAppendBlob(containerName, blobName));

    var writable = fs.createWriteStream(destinationFileNameTarget);
    blobService.createReadStream(containerName, blobName).pipe(writable);

    writeable.on('finish', function(){  
        var twitterImage = require('fs').readFileSync(destinationFileNameTarget);

        // Make post request on media endpoint. Pass file data as media parameter
        client.post('media/upload', {media: twitterImage}, function(error, media, response) {
            if (!error) {
                // If successful, a media object will be returned.
                context.log('media uploaded', media);

                // Lets tweet it
                var status = {
                    status: 'I am a tweet',
                    media_ids: media.media_id_string // Pass the media id string
                }

                client.post('statuses/update', status, function(error, tweet, response) {
                    if (!error) {
                        console.log('tweet sent', tweet);
                        console.log('tweet response', response);
                        blobSvc.deleteBlob(containerName, blobName, function(error, response){
                            if(!error){
                                // Blob has been deleted
                                context.log('deleted');
                                context.done();
                            } else {
                                context.log('error deleting');
                                context.done();
                            }
                        });
                    }
                });
            } else {
                context.log('error uploading', error);
                context.done();
            }
        });
    });
};