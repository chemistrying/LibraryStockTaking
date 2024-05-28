<template>
    <div class="col d-flex align-items-center">
        <div class="box w-100">
            <h5> Preview: {{ prefix + barcode + suffix }}</h5>
        </div>
    </div>
    <form>
        <div class="row g-4 mb-3 row-cols-1 row-cols-lg-5">
            <div class="col">
                <input type="text" v-model="prefix" class="form-control" :disabled="formDisableClass" placeholder="Prefix Format" aria-label="Prefix Format">
            </div>
            <div class="col">
                <input type="text" v-model="suffix" class="form-control" :disabled="formDisableClass" placeholder="Suffix Format" aria-label="Suffix Format">
            </div>
            <div class="col">
                <input type="text" v-model="barcode" class="form-control" :disabled="formDisableClass" placeholder="Barcode" aria-label="Last name">
            </div>
            <div class="col">
                <button type="submit" class="d-flex h-100 align-items-center btn btn-primary" :class="activeClass" @click.prevent="stocktakeBook">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-check" viewBox="0 0 16 16">
                        <path d="M10.97 4.97a.75.75 0 0 1 1.07 1.05l-3.99 4.99a.75.75 0 0 1-1.08.02L4.324 8.384a.75.75 0 1 1 1.06-1.06l2.094 2.093 3.473-4.425z"/>
                    </svg>
                </button>
            </div>
        </div>
    </form>
</template>

<script>
export default {
    props: ['session', 'bookshelf'],
    computed: {
        activeClass() {
            return {
                disabled: this.bookshelf.status == 3 || this.bookshelf.status == 0
            }
        },
        formDisableClass() {
            return this.bookshelf.status == 3 || this.bookshelf.status == 0
        }
    },
    data() {
        return {
            prefix: '',
            barcode: '',
            suffix: ''
        }
    },
    methods: {
        async stocktakeBook() {
            var finalBarcode = this.prefix + this.barcode + this.suffix;
            console.log(finalBarcode);

            var data = {
                sessionId: this.session.id,
                bookshelfId: this.bookshelf.id,
                operation: this.$root.operations.ADD,
                barcode: finalBarcode,
            }

            console.log(data);

            var response = await fetch(`${this.$root.apiUrl}/api/stocktake`, {
                method: "PATCH",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(data)
            });

            console.log(response.json());

            this.barcode = '';
            this.$parent.refreshBookshelf();
        }
    }
}
</script>