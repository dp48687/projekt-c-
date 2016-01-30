package com.example.danijel.kaladont;
import android.os.Parcel;
import android.os.Parcelable;

import microsoft.aspnet.signalr.client.http.android.AndroidPlatformComponent;
import microsoft.aspnet.signalr.client.Platform;
import microsoft.aspnet.signalr.client.hubs.HubConnection;
import microsoft.aspnet.signalr.client.hubs.HubProxy;

/**
 * Created by Danijel on 1/23/2016.
 */
/**
 * Manages the connection for a client and handles the transport of the strings
 * */
public class UserPlayManager implements Parcelable{
    private HubConnection connection;
    private HubProxy hubProxy;
    private PlayerStatus playerStatus = PlayerStatus.NOT_PLAYING;
    private Parcelable.Creator CREATOR;


    /**
     * The constructor initializes the connection
     * */
    public UserPlayManager() {
        connection = new HubConnection(""); // parametar je adresa servera
        connection.start();
        hubProxy = connection.createHubProxy("AdminHub");
    }

    /**
     * Used to invoke the server method <code>chooseString</code>
     * */
    public void sendString(String stringPlayed) {
        hubProxy.invoke("chooseString",stringPlayed);
    }

    /**
     * Invokes the <code>joinGame</code> from sever.
     * Invoked server method adds the player to the specified group.
     * */
    public void joinGame(String name) {
        hubProxy.invoke("joinGame",name);
    }

    /**
     * Invokes the <code>hostGame</code> from sever.
     * Invoked server method makes a new group which will contain players that will play against each other
     * */
    public void hostGame(boolean isCro) {
        hubProxy.invoke("hostGame",isCro);
    }

    /**
     * Invokes the <code>startGame</code> from the server
     * */
    public void startGame(String groupName) {
        hubProxy.invoke("startGame",groupName);
    }

    /**
     * Gets the database content by invoking a server method.
     * */
    public void getDatabaseContent() {

    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel dest, int flags) {

    }
}
