package com.midas.supernavi;

import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.SeekBar;

import com.h6ah4i.android.widget.verticalseekbar.VerticalSeekBar;

public class ProductSelection extends AppCompatActivity {

    private VerticalSeekBar modeSelector;
    private OperatingMode currentOperatingMode;


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
                case 2:
                    currentOperatingMode = OperatingMode.FREE_ROAM;
            }
        }

        public void onStartTrackingTouch(SeekBar seekBar) {}

        public void onStopTrackingTouch(SeekBar seekBar) {}

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
