package com.example.danijel.kaladont;

import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.View;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.Spinner;

public class SearchGames extends AppCompatActivity {
    private UserPlayManager userPlayManager;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_search_games);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        Bundle b = this.getIntent().getExtras();
        if (b!= null) {
            userPlayManager = b.getParcelable("playManager");
        }

        ImageButton button = (ImageButton)findViewById(R.id.imageButton5);
        Spinner spinner = (Spinner)findViewById(R.id.spinner);

        button.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

            }
        });
    }

    /**
     * Invokes the server method <code>joinGame</code>
     * */
    private void joinGame() {
        userPlayManager.joinGame("");   //parametar je ime grupe, a imena grupa su prirodni brojevi(indeksacija krece od 0)
    }

}
