(function ($) {

    const changeValidationRules = (form, prefix, oldIndex, newIndex) => {
        const rules = $(form).data("validator").settings.rules;
        for (let inputName in rules) {
            if (rules.hasOwnProperty(inputName) && inputName.startsWith(`${prefix}[${oldIndex}]`)) {
                const newInputName = inputName.replace(new RegExp(`[${oldIndex}]`, "g"), `${newIndex}`);
                rules[newInputName] = rules[inputName];
                delete rules[inputName];
            }
        }
    };

    const fixHiddenObjects = (container) => {
        const formObjects = container.find(".form-object");
        for (let i = 0; i < formObjects.length; i++) {
            const formObject = $(formObjects[i]);
            const inputs = formObject.find("input, select, textarea");
            for (let j = 0; j < inputs.length; j++) {
                const input = $(inputs[j]);
                const modifiedName = input.attr("name").replace(/\[\d+\]/, `[${i}]`);
                input.attr("name", modifiedName);
            }
            formObject.attr("data-index", i);
        }
    };

    const fixDisplayedObjectsList = (list) => {
        const objects = list.find(".removable-object");
        for (let i = 0; i < objects.length; i++) {
            $(objects[i]).attr("data-index", i);
        }
    };

    const setInputsIndex = (inputs, index) => {
        for (let i = 0; i < inputs.length; i++) {
            const input = $(inputs[i]);
            if (input.attr("name")) {
                const modifiedName = input.attr("name").replace(/\[\d+\]/, `[${index}]`);
                input.attr("name", modifiedName);
            }
        }
    };

    const setValidationSpansIndex = (labels, index) => {
        for (let i = 0; i < labels.length; i++) {
            const label = $(labels[i]);
            const modifiedName = label.attr("data-valmsg-for").replace(/\[\d+\]/, `[${index}]`);
            label.attr("data-valmsg-for", modifiedName);
        }
    };

    const removeBtnHandler = function () {
        const subForm = $(this).closest(".sub-form");
        const count = parseInt(subForm.attr("data-count"));
        const listItem = $(this).closest(".removable-object");
        const index = parseInt(listItem.attr("data-index"));
        const hiddenFormObject = subForm.find(`.form-object[data-index="${index}"]`)[0];
        listItem.remove();
        hiddenFormObject.remove();
        const hiddenContainer = $(subForm.find(".hidden-objects-container")[0]);
        fixHiddenObjects(hiddenContainer);
        const displayedObjectsList = $(subForm.find(".displayed-objects-list")[0]);
        fixDisplayedObjectsList(displayedObjectsList);
        const inputs = subForm.find(".form-inputs").find("input, select, textarea");
        const validationLabels = subForm.find(".validation-label");
        const newCount = count - 1;
        setInputsIndex(inputs, newCount);
        setValidationSpansIndex(validationLabels, newCount);
        const rulePrefix = subForm.attr("data-for");
        const form = subForm.closest("form");
        changeValidationRules(form, rulePrefix, count, newCount);
        subForm.attr("data-count", newCount);
    };

    const addToDisplayList = (list, displayFunction, count) => {
        const item = $(`<li class="removable-object" data-index=${count}></li>`);
        const displayStringDiv = $("<div class=\"col-md-10\"></div>");
        displayStringDiv.append(displayFunction());
        const removeBtnDiv = $("<div class=\"col-md-2 d-flex\"></div>");
        const removeBtn = $("<button type=\"button\" class=\"ml-auto btn-icon btn-sub-remove cancel\"></button>");
        removeBtn.click(removeBtnHandler);
        const removeSympol = $("<i class=\"fa fa-times\"></i>");
        const containerDiv = $("<div class=\"row info-item bg-ghostwhite-hover\"></div>");
        removeBtn.append(removeSympol);
        removeBtnDiv.append(removeBtn);
        containerDiv.append(displayStringDiv);
        containerDiv.append(removeBtnDiv);
        item.append(containerDiv);
        list.append(item);
    };

    const addToHiddenContainer = (container, inputs, count, name) => {
        const formObjectDiv = $(`<div class="form-object" data-index=${count}></div>`);
        for (let i = 0; i < inputs.length; i++) {
            const input = $(inputs[i]);
            const inputName = input.attr("name");
            if (input.attr("name")) {
                const hiddenInputName = inputName.replace(/.*\./, `${name}[${count}].`);
                console.log(hiddenInputName);
                const hiddenInput = $("<input>").attr("name", hiddenInputName).attr("value", input.val());
                formObjectDiv.append(hiddenInput);
            }
            input.val(null);
        }
        container.append(formObjectDiv);
    };

    const validateInputs = (inputs) => {
        let valid = true;
        for (let i = 0; i < inputs.length; i++) {
            valid = valid && $(inputs[i]).valid();
        }
        return valid;
    };

    $.fn.subform = function (displayFunction) {
        const submitButton = this.find(".btn-sub-submit");
        const inputsContainer = this.find(".form-inputs");
        const inputs = inputsContainer.find("input, select, textarea");
        const name = this.attr("data-for");
        console.log(name);

        $(submitButton).click(() => {
            if (validateInputs(inputs)) {
                const displayedList = $(this.find(".displayed-objects-list")[0]);
                const hiddenContainer = $(this.find(".hidden-objects-container")[0]);

                const count = parseInt(this.attr("data-count"));

                addToDisplayList(displayedList, displayFunction, count);
                addToHiddenContainer(hiddenContainer, inputs, count, name);
                
                //const validationLabels = this.find(".validation-label");
                const newCount = count + 1;
                //setValidationSpansIndex(validationLabels, newCount);

                //const rulePrefix = this.attr("data-for");
                //const form = this.closest("form");
                //changeValidationRules(form, rulePrefix, count, newCount);
                this.attr("data-count", newCount);
            }
        });
        this.find(".btn-sub-remove").click(removeBtnHandler);
        return this;
    };
}(jQuery));