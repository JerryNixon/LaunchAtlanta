var azure = require('azure-storage');
var fs = require("fs");
var Twitter = require('twitter');
var path = require('path');
var os = require('os');

module.exports = function (context, myBlob) {

    console.log("Starting console logging");

    var containerName = "workitems";
    var destinationFileNameTarget = path.join(os.tmpdir(),"file.jpg");

    var client = new Twitter({
        consumer_key: process.env.TWITTER_CONSUMER_KEY,
        consumer_secret: process.env.TWITTER_CONSUMER_SECRET,
        access_token_key: process.env.TWITTER_ACCESS_TOKEN_KEY,
        access_token_secret: process.env.TWITTER_ACCESS_TOKEN_SECRET
    });

    var blobName = context.bindingData.name;
    var blobSvc = azure.createBlobService();
    
    var writeable = fs.createWriteStream(destinationFileNameTarget);
    blobSvc.createReadStream(containerName, blobName).pipe(writeable);
    writeable.on('finish', function(){  
        var twitterImage = require('fs').readFileSync(destinationFileNameTarget);

        // Make post request on media endpoint. Pass file data as media parameter
        client.post('media/upload', {media: twitterImage}, function(error, media, response) {
            context.log("Begin Media Upload");
            if (!error) {
                // If successful, a media object will be returned.
                context.log('upload successful');

                // Lets tweet it
                var status = {
                    status: 'I am a tweet',
                    media_ids: media.media_id_string // Pass the media id string
                };

                context.log("twitter status to be processed");
                context.log(JSON.stringify(status));
                client.post('statuses/update', status, function(error, tweet, response) {
                    context.log("Tweet Updated");
                    context.log(JSON.stringify(tweet));
                    if (!error) {
                        var tweet_link = "https://twitter.com/MicrosoftLaunch/status/" + tweet[0].d_str;

                        context.log("Tweet sent");
                        context.log('Tweet link', tweet_link);

                        blobSvc.deleteBlob(containerName, blobName, function(error, response){
                            context.log("Begin deleting Blob");
                            if(!error){
                                // Blob has been deleted
                                context.log('Deleted');
                                context.done();
                            } else {
                                context.log('Error Deleting', error);
                                context.done();
                            }
                        });
                    } else {
                        console.log('Error send tweet', error);
                        context.done();
                    }
                });
            } else {
                context.log('Error Uploading', error);
                context.done();
            }
        });
    });
};