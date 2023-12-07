To to..


-fix element lifetime calculation.
		-lifetime = 0 error ,
		-liftime > 0.5 of total error.
		-one element lifetime , other no lifetime error.
		+++


-show totals in assemble model , call results component "detaild results. "

-add totals to model , so when filterd  you can stille get percentages.  or keep 2 data tables. ,, 


-construct elemnt from weight.


-geomerty input for elements ?
		-for wisulizations.
		--can geo id be used?  

		ReferenceID ?
			protected override void SolveInstance(IGH_DataAccess DA) {
    			Grasshopper.Kernel.Types.IGH_GeometricGoo geom = new Grasshopper.Kernel.Types.IGH_GeometricGoo();
    			DA.GetData(0, geom);
    			System.Guid id = geom.ReferenceID;
    
    			DA.SetData(0, id);
			}



			output of elements sais gwp /m3  shoud be just GWP. and elements can say a1 to a4


			a4 b3 d is per kg ?? is this correct? 


			filter model does not update on dissconnect


			add warning on filter not found ..


			add Karamba name....
