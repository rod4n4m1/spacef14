#/bin/bash
input=template_plasm_shot.png
degree=2
extension="18x18"
tile="18x10"
geometry="+0+0"
echo -n "."
cp $input plasm_shot000.png
while [ $degree -lt 360 ]
do
	if [ $degree -lt 10 ]
	then
		output="plasm_shot00${degree}.png"
	elif [ $degree -lt 100 ]
	then
		output="plasm_shot0${degree}.png"
	else
		output="plasm_shot${degree}.png"
	fi	
	echo -n "." 
	sips -r $degree --padColor 000000 $input --out $output &> /dev/null
	((degree=degree+2))
done
for fn in `ls plasm_shot*.png`; do
	convert $fn -background none -gravity center -extent $extension $fn 
done
montage plasm_shot*.png -tile $tile -geometry $geometry -background none montage_plasm.png
echo "."
