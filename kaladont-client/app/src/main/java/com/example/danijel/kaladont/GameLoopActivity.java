package com.example.danijel.kaladont;

import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

public class GameLoopActivity extends AppCompatActivity {
    public static UserPlayManager userPlayManager;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_game_loop);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        Bundle b = this.getIntent().getExtras();
        if (b!= null) {
            userPlayManager = b.getParcelable("playManager");
        }

        Button buttonOK = (Button)findViewById(R.id.button1);
        buttonOK.setOnClickListener(new Button.OnClickListener() {
            @Override
            public void onClick(View view) {
                sendString();
            }
        });
    }

    /**
     * Invokes server method <code>chooseString</code>
     * */
    private void sendString() {
        TextView userInput = (TextView)findViewById(R.id.textView1);
        String inputtedString = userInput.getText().toString();
        userPlayManager.sendString(inputtedString);
        userInput.setText("");
    }

}
