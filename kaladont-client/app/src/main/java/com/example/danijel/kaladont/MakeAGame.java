package com.example.danijel.kaladont;

import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.View;

public class MakeAGame extends AppCompatActivity {
    private UserPlayManager userPlayManager;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_make_agame);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        Bundle b = this.getIntent().getExtras();
        if (b!= null) {
            userPlayManager = b.getParcelable("playManager");
        }

    }

    /**
     * Calls the server method <code>hostGame</code>
     * */
    private void hostGame() {
        userPlayManager.hostGame(true);     // trebalo bi napraviti mogucnost da se dohvati podatak o parametru
    }

}
