//
// Text storage.
//

$TextTransfer_awaitingResponse = false;
$TextTransfer_buffer = ""; //Max character length in TGEA is 5k characters. Less than that can be sent, so we should be good.

function textTransfer_populateBuffer(%text)
{
    if($TextTransfer_awaitingResponse && strlen(%text) <= 4845) //`strlen` here is just a sanity check.
    {
        $TextTransfer_buffer = %text;
        return 1;
    }
    else
    {
        return 0;
    }
}

//For compiling the end result of `SendText` command into a single string.
function textTransfer_combineArgsToString(%a, %b, %c, %d, %e, %f, %g, %h, %i, %j, %k, %l, %m, %n, %o, %p, %q, %r, %s)
{
    return %a @ %b @ %c @ %d @ %e @ %f @ %g @ %h @ %i @ %j @ %k @ %l @ %m @ %n @ %o @ %p @ %q @ %r @ %s;
}

//
// Handshake functions.
//

function serverCmdCanSendText(%client)
{
    return !$TextTransfer_awaitingResponse;
}

function clientCmdCanSendText()
{
    return !$TextTransfer_awaitingResponse;
}

//
// Sending and receiving text.
//

//
// In terms of throughput, a serverCmd command can have up to 19 parameters, each able to hold a string 255 characters in length.
// Take off a single argument to store the client sending the information and you get a max character length of 4590, or a little over 4 KB.
// It's worth noting that clientCmd commands do not need to have a client specified, so their max character length goes up to 4845, nearly 5 KB.
//

function serverCmdSendText(%client, %a, %b, %c, %d, %e, %f, %g, %h, %i, %j, %k, %l, %m, %n, %o, %p, %q, %r)
{
    %builtString = textTransfer_combineArgsToString(%a, %b, %c, %d, %e, %f, %g, %h, %i, %j, %k, %l, %m, %n, %o, %p, %q, %r);
    return textTransfer_populateBuffer(%builtString);
}

function clientCmdSendText(%a, %b, %c, %d, %e, %f, %g, %h, %i, %j, %k, %l, %m, %n, %o, %p, %q, %r, %s)
{
    %builtString = textTransfer_combineArgsToString(%a, %b, %c, %d, %e, %f, %g, %h, %i, %j, %k, %l, %m, %n, %o, %p, %q, %r, %s);
    return textTransfer_populateBuffer(%builtString);
}

//
// Sending files.
//

function textTransfer_sendTextFileToClient(%client, %fileName)
{
    %filePath = expandFilename(%fileName);
    if(!isFile(%filePath))
    {
        //Can't send a file that doesn't exist.
        return 0;
    }
    else if(getFileLength(%filePath) > 4845)
    {
        //Don't send a file if it's too big.
        return 0;
    }

    %textToSend = "";

    //Read the file and extract the text.
    %file = new fileObject();
    %file.openForRead(%filePath);
    while(!%file.isEOF())
    {
        %textToSend = %textToSend @ %file.readLine();
    }

    //Split up file into 255-character arguments to send to the client.
    %a[0] = ""; //A is for arguments.
    %argumentsNeeded = mCeil(strlen(%textToSend) / 255);
    for(%i = 0; %i < %argumentsNeeded; %i++)
    {
        if(%i == 0)
        {
            %startCharacterIndex = 0;
        }
        else
        {
            %startCharacterIndex = 1 + (255 * %i);
        }
        
        %a[%i] = getSubStr(%textToSend, %startCharacterIndex, 255);
    }

    //Based on how many arguments resulted, send the text to the client.
    //The stairway to the heavens...
    switch(%argumentsNeeded)
    {
        case 1:
            return commandToClient(%client, 'SendText', %a[0]);
        case 2:
            return commandToClient(%client, 'SendText', %a[0], %a[1]);
        case 3:
            return commandToClient(%client, 'SendText', %a[0], %a[1], %a[2]);
        case 4:
            return commandToClient(%client, 'SendText', %a[0], %a[1], %a[2], %a[3]);
        case 5:
            return commandToClient(%client, 'SendText', %a[0], %a[1], %a[2], %a[3], %a[4]);
        case 6:
            return commandToClient(%client, 'SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5]);
        case 7:
            return commandToClient(%client, 'SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6]);
        case 8:
            return commandToClient(%client, 'SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7]);
        case 9:
            return commandToClient(%client, 'SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8]);
        case 10:
            return commandToClient(%client, 'SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9]);
        case 11:
            return commandToClient(%client, 'SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9], %a[10]);
        case 12:
            return commandToClient(%client, 'SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9], %a[10], %a[11]);
        case 13:
            return commandToClient(%client, 'SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9], %a[10], %a[11], %a[12]);
        case 14:
            return commandToClient(%client, 'SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9], %a[10], %a[11], %a[12], %a[13]);
        case 15:
            return commandToClient(%client, 'SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9], %a[10], %a[11], %a[12], %a[13], %a[14]);
        case 16:
            return commandToClient(%client, 'SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9], %a[10], %a[11], %a[12], %a[13], %a[14], %a[15]);
        case 17:
            return commandToClient(%client, 'SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9], %a[10], %a[11], %a[12], %a[13], %a[14], %a[15], %a[16]);
        case 18:
            return commandToClient(%client, 'SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9], %a[10], %a[11], %a[12], %a[13], %a[14], %a[15], %a[16], %a[17]);
        case 19:
            return commandToClient(%client, 'SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9], %a[10], %a[11], %a[12], %a[13], %a[14], %a[15], %a[16], %a[17], %a[18]);
        default:
            return 0;
    }
}

function textTransfer_sendTextFileToServer(%fileName)
{
    %filePath = expandFilename(%fileName);
    if(!isObject(ServerConnection))
    {
        //Can't send a file to the server if it doesn't exist.
        return 0;
    }
    else if(!isFile(%filePath))
    {
        //Can't send a file that doesn't exist.
        return 0;
    }
    else if(getFileLength(%filePath) > 4590)
    {
        //Don't send a file if it's too big.
        return 0;
    }

    %textToSend = "";

    //Read the file and extract the text.
    %file = new fileObject();
    %file.openForRead(%filePath);
    while(!%file.isEOF())
    {
        %textToSend = %textToSend @ %file.readLine();
    }

    //Split up file into 255-character arguments to send to the client.
    %a[0] = ""; //A is for arguments.
    %argumentsNeeded = mCeil(strlen(%textToSend) / 255);
    for(%i = 0; %i < %argumentsNeeded; %i++)
    {
        if(%i == 0)
        {
            %startCharacterIndex = 0;
        }
        else
        {
            %startCharacterIndex = 1 + (255 * %i);
        }
        
        %a[%i] = getSubStr(%textToSend, %startCharacterIndex, 255);
    }

    //Based on how many arguments resulted, send the text to the server.
    //The stairway to success!
    switch(%argumentsNeeded)
    {
        case 1:
            return commandToServer('SendText', %a[0]);
        case 2:
            return commandToServer('SendText', %a[0], %a[1]);
        case 3:
            return commandToServer('SendText', %a[0], %a[1], %a[2]);
        case 4:
            return commandToServer('SendText', %a[0], %a[1], %a[2], %a[3]);
        case 5:
            return commandToServer('SendText', %a[0], %a[1], %a[2], %a[3], %a[4]);
        case 6:
            return commandToServer('SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5]);
        case 7:
            return commandToServer('SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6]);
        case 8:
            return commandToServer('SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7]);
        case 9:
            return commandToServer('SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8]);
        case 10:
            return commandToServer('SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9]);
        case 11:
            return commandToServer('SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9], %a[10]);
        case 12:
            return commandToServer('SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9], %a[10], %a[11]);
        case 13:
            return commandToServer('SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9], %a[10], %a[11], %a[12]);
        case 14:
            return commandToServer('SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9], %a[10], %a[11], %a[12], %a[13]);
        case 15:
            return commandToServer('SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9], %a[10], %a[11], %a[12], %a[13], %a[14]);
        case 16:
            return commandToServer('SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9], %a[10], %a[11], %a[12], %a[13], %a[14], %a[15]);
        case 17:
            return commandToServer('SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9], %a[10], %a[11], %a[12], %a[13], %a[14], %a[15], %a[16]);
        case 18:
            return commandToServer('SendText', %a[0], %a[1], %a[2], %a[3], %a[4], %a[5], %a[6], %a[7], %a[8], %a[9], %a[10], %a[11], %a[12], %a[13], %a[14], %a[15], %a[16], %a[17]);
        default:
            return 0;
    }
}