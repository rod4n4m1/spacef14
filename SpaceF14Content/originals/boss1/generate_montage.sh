#/bin/bash
tile="3x2"
geometry="+0+0"
echo -n "."
montage boss0*.png -tile $tile -geometry $geometry -background none montage_boss1.png
echo "."
