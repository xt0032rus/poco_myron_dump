#! /vendor/bin/sh
#=============================================================================
# Copyright (c) 2022 Qualcomm Technologies, Inc.
# All Rights Reserved.
# Confidential and Proprietary - Qualcomm Technologies, Inc.
#=============================================================================

SYSTEM_DLKM_DIR="/system_dlkm/lib/modules"
VENDOR_DLKM_DIR="/vendor_dlkm/lib/modules"

MODPROBE="/vendor/bin/modprobe"

for system_dlkm_dir in ${SYSTEM_DLKM_DIR} ;
do

	for kernel_ver_dir in "${system_dlkm_dir}" "${system_dlkm_dir}"/*;
	do

		if [ ! -e ${kernel_ver_dir}/modules.load ]; then
			continue
		fi

		echo "Loading module dir $kernel_ver_dir"

		if [ -e ${VENDOR_DLKM_DIR}/system_dlkm.modules.blocklist ] && grep -q blocklist ${VENDOR_DLKM_DIR}/system_dlkm.modules.blocklist; then
			blocklist_expr="$(sed -n -e 's/blocklist \(.*\)/\1/p' ${VENDOR_DLKM_DIR}/system_dlkm.modules.blocklist | sed -e 's/-/[-_]/g' -e 's/^/-e /')"
		else
			# Use pattern that won't be found in modules list so that all modules pass through grep below
			blocklist_expr="-e %"
		fi
		# Filter out modules in blocklist - we would see unnecessary errors otherwise
		load_modules=$(find ${kernel_ver_dir} -name "*.ko" | grep -w -v ${blocklist_expr})
		first_module=$(echo ${load_modules} | cut -d " " -f1)
		other_modules=$(echo ${load_modules} | cut -d " " -f2-)
		if ! ${MODPROBE} -b -s -d ${kernel_ver_dir} -a ${first_module} > /dev/null ; then
			echo "modprobe first module failed! skip!"
			continue
		fi
		# load modules individually in case one of them fails to init
		for module in ${other_modules}; do
			( ${MODPROBE} -b -s -d ${kernel_ver_dir} -a ${module} > /dev/null ) &
		done
		# Wait until current dir modprobe finished
		wait
	done

	wait

	exit 0
done

exit 1
