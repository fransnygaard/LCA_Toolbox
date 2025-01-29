download the sample file for instructions. 

https://github.com/fransnygaard/LCA_Toolbox/blob/master/SAMPLES/LCA%20SAMPLE.gh



![alt text](https://github.com/fransnygaard/LCA_Toolbox/blob/master/SAMPLES/01_Materials.png?raw=true)
![alt text](https://github.com/fransnygaard/LCA_Toolbox/blob/master/SAMPLES/02_Elements.png?raw=true)
![alt text](https://github.com/fransnygaard/LCA_Toolbox/blob/master/SAMPLES/03_Model.png?raw=true)
![alt text](https://github.com/fransnygaard/LCA_Toolbox/blob/master/SAMPLES/04_Results.png?raw=true)'

TO DO:
	

	-Custom Database.
		-Read from DB
			-If db path is changed.. reset dropdowns.


		-Add / modify
			-Check if db exists
				-if not create

		-

	-Filter results. 
		.





		FIX 21022024
			values not from db . 
						insulation  - fix in scraper


		Construct element output
				Add a4 to A1-A3

		
		RESULT Viewer ,
			ADD input colors
			ADD dropdown for individual elements , for element name , or elemenet gorup. 


		New features
			Add result totaling embodied and operational emmisions.


fix before 0.1.3 release. 


SCRAPER
-	GET INSULATION VALUE-

	MATERIAL-
	INSULATION NOT YET IPLEMENTED.
	 

	DONE, NOT TESTED . -element output change from a1-a3 to a1-to a4
	-all element constructors on the same level as solid.
	DONE, FIRST TEST LOOKS OK.- warning in visulizer if some elements dont have geo
	-Add not yet implemnted om D:reuse factor.
	-Add flatten to elements input? 
	A4 PER TONN? 

	-bug in "filter model" -"1. Solution exception:Column 'A1-A3_noSeq' does not belong to table .
	
	
	allow sqe input on assemble model does not change the GWP output on the same component
	looks like it always alows it ?? 

	ADD CSV OUTPUT TO TIMELINE


	TIMELINE 
		-flip Valuetree maxtrix
		-add csv output.


	VISUALIZER
		-if min and max is the same  , set all lambda to 0.5
		-looks like normalizer does not work correct.
		-dropdown , per element , group by name , group by category , group by material.
		-legend ? 
		-text overlay ? 

		
