package com.midas.supernavi;

import android.content.Intent;
import android.os.Bundle;
import android.speech.RecognizerIntent;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.SeekBar;

import com.h6ah4i.android.widget.verticalseekbar.VerticalSeekBar;

import java.util.List;

public class ProductSelection extends AppCompatActivity {

    private VerticalSeekBar modeSelector;
    private OperatingMode currentOperatingMode;

    private static final int SPEECH_REQUEST_CODE = 0;


    enum OperatingMode {
        PRODUCT_SELECTION,
        NAVIGATION,
        FREE_ROAM
    }

    class modeListener implements SeekBar.OnSeekBarChangeListener {

        public void onProgressChanged(SeekBar seekBar, int progress,
                                      boolean fromUser) {
            switch (progress){
                case 0:
                    currentOperatingMode = OperatingMode.PRODUCT_SELECTION;
                case 1:
                    currentOperatingMode = OperatingMode.NAVIGATION;
                    navigate();
                case 2:
                    currentOperatingMode = OperatingMode.FREE_ROAM;
                    freeRoam();

            }
        }
        public void onStartTrackingTouch(SeekBar seekBar) {}
        public void onStopTrackingTouch(SeekBar seekBar) {}

    }

    //Handles navigate mode
    private void navigate(){


    }


    //Handles navigate mode
    private void freeRoam(){


    }

    private void displaySpeechRecognizer() {
        Intent intent = new Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH);
        intent.putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL,
                RecognizerIntent.LANGUAGE_MODEL_FREE_FORM);
        // Start the activity, the intent will be populated with the speech text
        startActivityForResult(intent, SPEECH_REQUEST_CODE);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode,
                                    Intent data) {
        if (requestCode == SPEECH_REQUEST_CODE && resultCode == RESULT_OK) {
            List<String> results = data.getStringArrayListExtra(
                    RecognizerIntent.EXTRA_RESULTS);
            String spokenText = results.get(0);
            // Do something with spokenText
        }
        super.onActivityResult(requestCode, resultCode, data);
    }


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_product_selection);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        populateListView();
        //Default mode is product selection
        this.setTitle("SuperNavi - Product Selection");
        currentOperatingMode = OperatingMode.PRODUCT_SELECTION;
        modeSelector = (VerticalSeekBar) findViewById(R.id.modeSelector);
        modeSelector.setOnSeekBarChangeListener(new modeListener());

    }

    //Creates grocery list
    private void populateListView(){
        String[] groceries = {"Bananas","Milk","Steak","Lettuce","Chips","Bread"};
        ArrayAdapter<String> adapter = new ArrayAdapter<String>(this, R.layout.groceries, groceries);
        ListView list = (ListView) findViewById(R.id.groceryList);
        list.setAdapter(adapter);
    }



    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_product_selection, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }
}
