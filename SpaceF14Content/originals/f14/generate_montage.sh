#/bin/bash
input=template_f14_tomcat.png
degree=2
extension="100x100"
tile="18x10"
geometry="+0+0"
echo -n "."
cp $input f14_tomcat000.png
while [ $degree -lt 360 ]
do
	if [ $degree -lt 10 ]
	then
		output="f14_tomcat00${degree}.png"
	elif [ $degree -lt 100 ]
	then
		output="f14_tomcat0${degree}.png"
	else
		output="f14_tomcat${degree}.png"
	fi	
	echo -n "." 
	sips -r $degree --padColor 000000 $input --out $output &> /dev/null
	((degree=degree+2))
done
for fn in `ls f14_tomcat*.png`; do
	convert $fn -background none -gravity center -extent $extension $fn 
done
montage f14_tomcat*.png -tile $tile -geometry $geometry -background none montage_tomcat.png
echo "."
